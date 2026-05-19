#!/usr/bin/env bash
# Re-deploy del backend EduSpace a Azure Container Apps.
# Asume que los recursos ya existen (rg-eduspace-academic, ca1e77e4bf38acr,
# eduspace-api-env, eduspace-api). Si necesitás crearlos desde cero, mirá
# la sesión de migración en engram (topic config/backend-migrado-a-azure-container-apps).

set -euo pipefail

# --- Config (sobrescribir por env vars si hace falta) ---
RG="${RG:-rg-eduspace-academic}"
APP_NAME="${APP_NAME:-eduspace-api}"
ACR_NAME="${ACR_NAME:-ca1e77e4bf38acr}"
IMAGE_NAME="${IMAGE_NAME:-eduspace-api}"
IMAGE_TAG="${IMAGE_TAG:-v$(date -u +%Y%m%d%H%M%S)}"
PROJECT_DIR="${PROJECT_DIR:-$(dirname "$(realpath "$0")")}"

IMAGE_REF="${ACR_NAME}.azurecr.io/${IMAGE_NAME}:${IMAGE_TAG}"

# --- Sanity ---
command -v az >/dev/null || { echo "az CLI no encontrado"; exit 1; }
command -v docker >/dev/null || { echo "docker no encontrado"; exit 1; }
az account show >/dev/null 2>&1 || { echo "No estás logueado en az. Corré: az login"; exit 1; }
[[ -f "$PROJECT_DIR/Dockerfile" ]] || { echo "Dockerfile no encontrado en $PROJECT_DIR"; exit 1; }

echo "▶ Resource Group : $RG"
echo "▶ Container App  : $APP_NAME"
echo "▶ Registry       : $ACR_NAME.azurecr.io"
echo "▶ Image          : $IMAGE_REF"
echo

# --- 1. Login al registry ---
echo "[1/4] az acr login…"
az acr login -n "$ACR_NAME" >/dev/null

# --- 2. Build ---
echo "[2/4] docker build…"
docker build -t "$IMAGE_REF" "$PROJECT_DIR" | tail -5

# --- 3. Push ---
echo "[3/4] docker push…"
docker push "$IMAGE_REF" | tail -3

# --- 4. Update Container App ---
echo "[4/4] az containerapp update…"
FQDN=$(az containerapp update \
  -g "$RG" \
  -n "$APP_NAME" \
  --image "$IMAGE_REF" \
  --query "properties.configuration.ingress.fqdn" \
  -o tsv)

URL="https://${FQDN}"
echo
echo "✓ Deploy completo."
echo "  URL: $URL"
echo
echo "Smoke test:"
curl -sk -o /dev/null -w "  POST /api/v1/authentication/activate {token:''} → HTTP %{http_code}\n" \
  --max-time 30 -X POST "$URL/api/v1/authentication/activate" \
  -H "Content-Type: application/json" -d '{"token":""}'

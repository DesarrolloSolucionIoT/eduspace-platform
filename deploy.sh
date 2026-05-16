#!/usr/bin/env bash
# Re-deploy del backend EduSpace a Azure App Service.
# Asume que los recursos ya están creados (rg-eduspace-academic, app-eduspace-*).
# Si necesitas crearlos desde cero, mirá la sesión inicial de deployment en engram
# (topic_key: azure/deployment/backend) o el README.

set -euo pipefail

# --- Config (sobrescribir por env vars si hace falta) ---
RG="${RG:-rg-eduspace-academic}"
APP_NAME="${APP_NAME:-app-eduspace-7b4076}"
PROJECT_DIR="${PROJECT_DIR:-$(dirname "$(realpath "$0")")/FULLSTACKFURY.EduSpace.API}"
PUBLISH_DIR="$PROJECT_DIR/publish"
ZIP_PATH="$PROJECT_DIR/publish.zip"

# --- Sanity ---
command -v az >/dev/null || { echo "az CLI no encontrado"; exit 1; }
command -v dotnet >/dev/null || { echo "dotnet no encontrado"; exit 1; }
az account show >/dev/null 2>&1 || { echo "No estás logueado en az. Corré: az login"; exit 1; }
[[ -d "$PROJECT_DIR" ]] || { echo "PROJECT_DIR no existe: $PROJECT_DIR"; exit 1; }

echo "▶ Resource Group : $RG"
echo "▶ Web App        : $APP_NAME"
echo "▶ Project dir    : $PROJECT_DIR"
echo

# --- 1. Publish ---
echo "[1/3] dotnet publish (Release)…"
rm -rf "$PUBLISH_DIR" "$ZIP_PATH"
dotnet publish "$PROJECT_DIR" -c Release -o "$PUBLISH_DIR" --nologo | tail -5

# --- 2. Zip ---
echo "[2/3] Empaquetando zip…"
python3 - <<PY
import os, zipfile
base = "$PUBLISH_DIR"
out  = "$ZIP_PATH"
with zipfile.ZipFile(out, "w", zipfile.ZIP_DEFLATED) as z:
    for root, _, files in os.walk(base):
        for f in files:
            p = os.path.join(root, f)
            z.write(p, os.path.relpath(p, base))
print(f"  → {out} ({os.path.getsize(out)/1024/1024:.1f} MB)")
PY

# --- 3. Deploy ---
echo "[3/3] az webapp deploy…"
az webapp deploy \
  --resource-group "$RG" \
  --name "$APP_NAME" \
  --src-path "$ZIP_PATH" \
  --type zip \
  --async false \
  --output none

URL="https://${APP_NAME}.azurewebsites.net"
echo
echo "✓ Deploy completo."
echo "  Swagger: $URL/swagger/index.html"
echo
echo "Smoke test:"
curl -sk -o /dev/null -w "  /swagger/index.html → HTTP %{http_code}\n" --max-time 30 "$URL/swagger/index.html"

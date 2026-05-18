using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.BreakdownManagement.Domain.Model.ValueObjects;

public class ReportStatusTests
{
    [Fact]
    public void EnEspera_HasPendingValue()
    {
        // Arrange & Act & Assert
        ReportStatus.EnEspera.Value.Should().Be("pending");
    }

    [Fact]
    public void EnProceso_HasInProgressValue()
    {
        // Arrange & Act & Assert
        ReportStatus.EnProceso.Value.Should().Be("in progress");
    }

    [Fact]
    public void Completado_HasCompletedValue()
    {
        // Arrange & Act & Assert
        ReportStatus.Completado.Value.Should().Be("completed");
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("in progress")]
    [InlineData("completed")]
    public void FromString_KnownStatus_ReturnsCorrectInstance(string statusStr)
    {
        // Arrange & Act
        var status = ReportStatus.FromString(statusStr);

        // Assert
        status.Value.Should().Be(statusStr);
    }

    [Fact]
    public void FromString_PendingString_ReturnsEnEsperaInstance()
    {
        // Arrange & Act
        var status = ReportStatus.FromString("pending");

        // Assert
        status.Should().Be(ReportStatus.EnEspera);
    }

    [Fact]
    public void FromString_InProgressString_ReturnsEnProcesoInstance()
    {
        // Arrange & Act
        var status = ReportStatus.FromString("in progress");

        // Assert
        status.Should().Be(ReportStatus.EnProceso);
    }

    [Fact]
    public void FromString_CompletedString_ReturnsCompletadoInstance()
    {
        // Arrange & Act
        var status = ReportStatus.FromString("completed");

        // Assert
        status.Should().Be(ReportStatus.Completado);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("IN PROGRESS")]
    [InlineData("done")]
    [InlineData("")]
    [InlineData("unknown")]
    public void FromString_UnknownStatus_ThrowsInvalidReportDataException(string unknownStatus)
    {
        // Arrange & Act
        Action act = () => ReportStatus.FromString(unknownStatus);

        // Assert
        act.Should().Throw<InvalidReportDataException>()
            .WithMessage($"*{unknownStatus}*is not a valid report status*");
    }

    [Fact]
    public void TwoEnEsperaInstances_AreEqual()
    {
        // Arrange & Act & Assert
        ReportStatus.EnEspera.Should().Be(ReportStatus.EnEspera);
    }

    [Fact]
    public void EnEspera_AndEnProceso_AreNotEqual()
    {
        // Arrange & Act & Assert
        ReportStatus.EnEspera.Should().NotBe(ReportStatus.EnProceso);
    }
}

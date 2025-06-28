using Scandalous.Core.Enums;
using Xunit;

namespace Scandalous.Core.Tests.Enums
{
    public class ScannerColorModeTests
    {
        [Fact]
        public void ScannerColorMode_ContainsExpectedValues()
        {
            // Arrange & Act
            var values = Enum.GetValues<ScannerColorMode>();

            // Assert
            Assert.Equal(3, values.Length);
            Assert.Contains(ScannerColorMode.Grayscale, values);
            Assert.Contains(ScannerColorMode.BlackAndWhite, values);
            Assert.Contains(ScannerColorMode.Color, values);
        }

        [Fact]
        public void ScannerColorMode_ValuesHaveCorrectUnderlyingIntegers()
        {
            // Assert
            Assert.Equal(0, (int)ScannerColorMode.Grayscale);
            Assert.Equal(1, (int)ScannerColorMode.BlackAndWhite);
            Assert.Equal(2, (int)ScannerColorMode.Color);
        }

        [Fact]
        public void ScannerColorMode_CanBeParsedFromString()
        {
            // Arrange & Act & Assert
            Assert.Equal(ScannerColorMode.Grayscale, Enum.Parse<ScannerColorMode>("Grayscale"));
            Assert.Equal(ScannerColorMode.BlackAndWhite, Enum.Parse<ScannerColorMode>("BlackAndWhite"));
            Assert.Equal(ScannerColorMode.Color, Enum.Parse<ScannerColorMode>("Color"));
        }

        [Fact]
        public void ScannerColorMode_CanBeParsedFromInteger()
        {
            // Arrange & Act & Assert
            Assert.Equal(ScannerColorMode.Grayscale, (ScannerColorMode)0);
            Assert.Equal(ScannerColorMode.BlackAndWhite, (ScannerColorMode)1);
            Assert.Equal(ScannerColorMode.Color, (ScannerColorMode)2);
        }

        [Fact]
        public void ScannerColorMode_ToString_ReturnsCorrectValue()
        {
            // Arrange & Act & Assert
            Assert.Equal("Grayscale", ScannerColorMode.Grayscale.ToString());
            Assert.Equal("BlackAndWhite", ScannerColorMode.BlackAndWhite.ToString());
            Assert.Equal("Color", ScannerColorMode.Color.ToString());
        }

        [Fact]
        public void ScannerColorMode_CanBeUsedInSwitchExpression()
        {
            // Arrange
            var colorMode = ScannerColorMode.Color;

            // Act
            var result = colorMode switch
            {
                ScannerColorMode.Grayscale => "gray",
                ScannerColorMode.BlackAndWhite => "bw",
                ScannerColorMode.Color => "color",
                _ => "unknown"
            };

            // Assert
            Assert.Equal("color", result);
        }

        [Fact]
        public void ScannerColorMode_CanBeUsedInSwitchStatement()
        {
            // Arrange
            var colorMode = ScannerColorMode.BlackAndWhite;
            var result = "";

            // Act
            switch (colorMode)
            {
                case ScannerColorMode.Grayscale:
                    result = "gray";
                    break;
                case ScannerColorMode.BlackAndWhite:
                    result = "bw";
                    break;
                case ScannerColorMode.Color:
                    result = "color";
                    break;
                default:
                    result = "unknown";
                    break;
            }

            // Assert
            Assert.Equal("bw", result);
        }

        [Fact]
        public void ScannerColorMode_CanBeCompared()
        {
            // Arrange
            var grayscale = ScannerColorMode.Grayscale;
            var blackAndWhite = ScannerColorMode.BlackAndWhite;
            var color = ScannerColorMode.Color;

            // Act & Assert
            Assert.True(grayscale < blackAndWhite);
            Assert.True(blackAndWhite < color);
            Assert.True(grayscale < color);
            Assert.True(color > blackAndWhite);
            Assert.True(blackAndWhite > grayscale);
        }

        [Fact]
        public void ScannerColorMode_CanBeUsedInCollections()
        {
            // Arrange
            var modes = new List<ScannerColorMode>
            {
                ScannerColorMode.Grayscale,
                ScannerColorMode.BlackAndWhite,
                ScannerColorMode.Color
            };

            // Act & Assert
            Assert.Equal(3, modes.Count);
            Assert.Contains(ScannerColorMode.Grayscale, modes);
            Assert.Contains(ScannerColorMode.BlackAndWhite, modes);
            Assert.Contains(ScannerColorMode.Color, modes);
        }

        [Fact]
        public void ScannerColorMode_CanBeUsedInDictionary()
        {
            // Arrange
            var modeDescriptions = new Dictionary<ScannerColorMode, string>
            {
                { ScannerColorMode.Grayscale, "Gray scale scanning" },
                { ScannerColorMode.BlackAndWhite, "Black and white scanning" },
                { ScannerColorMode.Color, "Color scanning" }
            };

            // Act & Assert
            Assert.Equal(3, modeDescriptions.Count);
            Assert.Equal("Gray scale scanning", modeDescriptions[ScannerColorMode.Grayscale]);
            Assert.Equal("Black and white scanning", modeDescriptions[ScannerColorMode.BlackAndWhite]);
            Assert.Equal("Color scanning", modeDescriptions[ScannerColorMode.Color]);
        }

        [Fact]
        public void ScannerColorMode_CanBeSerialized()
        {
            // Arrange
            var colorMode = ScannerColorMode.Color;

            // Act
            var serialized = colorMode.ToString();

            // Assert
            Assert.Equal("Color", serialized);
        }

        [Fact]
        public void ScannerColorMode_CanBeDeserialized()
        {
            // Arrange
            var serialized = "BlackAndWhite";

            // Act
            var deserialized = Enum.Parse<ScannerColorMode>(serialized);

            // Assert
            Assert.Equal(ScannerColorMode.BlackAndWhite, deserialized);
        }

        [Theory]
        [InlineData(ScannerColorMode.Grayscale, "Grayscale")]
        [InlineData(ScannerColorMode.BlackAndWhite, "BlackAndWhite")]
        [InlineData(ScannerColorMode.Color, "Color")]
        public void ScannerColorMode_ToString_ReturnsExpectedValue(ScannerColorMode mode, string expected)
        {
            // Act
            var result = mode.ToString();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Grayscale", ScannerColorMode.Grayscale)]
        [InlineData("BlackAndWhite", ScannerColorMode.BlackAndWhite)]
        [InlineData("Color", ScannerColorMode.Color)]
        public void ScannerColorMode_Parse_ReturnsExpectedValue(string value, ScannerColorMode expected)
        {
            // Act
            var result = Enum.Parse<ScannerColorMode>(value);

            // Assert
            Assert.Equal(expected, result);
        }
    }
} 
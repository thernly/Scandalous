using Scandalous.Core.Enums;
using Xunit;

namespace Scandalous.Core.Tests.Enums
{
    public class ScannerPaperSourceTests
    {
        [Fact]
        public void ScannerPaperSource_ContainsExpectedValues()
        {
            // Arrange & Act
            var values = Enum.GetValues<ScannerPaperSource>();

            // Assert
            Assert.Equal(4, values.Length);
            Assert.Contains(ScannerPaperSource.Auto, values);
            Assert.Contains(ScannerPaperSource.Flatbed, values);
            Assert.Contains(ScannerPaperSource.FeederDuplex, values);
            Assert.Contains(ScannerPaperSource.FeederSimplex, values);
        }

        [Fact]
        public void ScannerPaperSource_ValuesHaveCorrectUnderlyingIntegers()
        {
            // Assert
            Assert.Equal(0, (int)ScannerPaperSource.Auto);
            Assert.Equal(1, (int)ScannerPaperSource.Flatbed);
            Assert.Equal(2, (int)ScannerPaperSource.FeederDuplex);
            Assert.Equal(3, (int)ScannerPaperSource.FeederSimplex);
        }

        [Fact]
        public void ScannerPaperSource_CanBeParsedFromString()
        {
            // Arrange & Act & Assert
            Assert.Equal(ScannerPaperSource.Auto, Enum.Parse<ScannerPaperSource>("Auto"));
            Assert.Equal(ScannerPaperSource.Flatbed, Enum.Parse<ScannerPaperSource>("Flatbed"));
            Assert.Equal(ScannerPaperSource.FeederDuplex, Enum.Parse<ScannerPaperSource>("FeederDuplex"));
            Assert.Equal(ScannerPaperSource.FeederSimplex, Enum.Parse<ScannerPaperSource>("FeederSimplex"));
        }

        [Fact]
        public void ScannerPaperSource_CanBeParsedFromInteger()
        {
            // Arrange & Act & Assert
            Assert.Equal(ScannerPaperSource.Auto, (ScannerPaperSource)0);
            Assert.Equal(ScannerPaperSource.Flatbed, (ScannerPaperSource)1);
            Assert.Equal(ScannerPaperSource.FeederDuplex, (ScannerPaperSource)2);
            Assert.Equal(ScannerPaperSource.FeederSimplex, (ScannerPaperSource)3);
        }

        [Fact]
        public void ScannerPaperSource_ToString_ReturnsCorrectValue()
        {
            // Arrange & Act & Assert
            Assert.Equal("Auto", ScannerPaperSource.Auto.ToString());
            Assert.Equal("Flatbed", ScannerPaperSource.Flatbed.ToString());
            Assert.Equal("FeederDuplex", ScannerPaperSource.FeederDuplex.ToString());
            Assert.Equal("FeederSimplex", ScannerPaperSource.FeederSimplex.ToString());
        }

        [Fact]
        public void ScannerPaperSource_CanBeUsedInSwitchExpression()
        {
            // Arrange
            var paperSource = ScannerPaperSource.Flatbed;

            // Act
            var result = paperSource switch
            {
                ScannerPaperSource.Auto => "auto",
                ScannerPaperSource.Flatbed => "flatbed",
                ScannerPaperSource.FeederDuplex => "duplex",
                ScannerPaperSource.FeederSimplex => "simplex",
                _ => "unknown"
            };

            // Assert
            Assert.Equal("flatbed", result);
        }

        [Fact]
        public void ScannerPaperSource_CanBeUsedInSwitchStatement()
        {
            // Arrange
            var paperSource = ScannerPaperSource.FeederDuplex;
            var result = "";

            // Act
            switch (paperSource)
            {
                case ScannerPaperSource.Auto:
                    result = "auto";
                    break;
                case ScannerPaperSource.Flatbed:
                    result = "flatbed";
                    break;
                case ScannerPaperSource.FeederDuplex:
                    result = "duplex";
                    break;
                case ScannerPaperSource.FeederSimplex:
                    result = "simplex";
                    break;
                default:
                    result = "unknown";
                    break;
            }

            // Assert
            Assert.Equal("duplex", result);
        }

        [Fact]
        public void ScannerPaperSource_CanBeCompared()
        {
            // Arrange
            var auto = ScannerPaperSource.Auto;
            var flatbed = ScannerPaperSource.Flatbed;
            var duplex = ScannerPaperSource.FeederDuplex;
            var simplex = ScannerPaperSource.FeederSimplex;

            // Act & Assert
            Assert.True(auto < flatbed);
            Assert.True(flatbed < duplex);
            Assert.True(duplex < simplex);
            Assert.True(simplex > duplex);
            Assert.True(duplex > flatbed);
            Assert.True(flatbed > auto);
        }

        [Fact]
        public void ScannerPaperSource_CanBeUsedInCollections()
        {
            // Arrange
            var sources = new List<ScannerPaperSource>
            {
                ScannerPaperSource.Auto,
                ScannerPaperSource.Flatbed,
                ScannerPaperSource.FeederDuplex,
                ScannerPaperSource.FeederSimplex
            };

            // Act & Assert
            Assert.Equal(4, sources.Count);
            Assert.Contains(ScannerPaperSource.Auto, sources);
            Assert.Contains(ScannerPaperSource.Flatbed, sources);
            Assert.Contains(ScannerPaperSource.FeederDuplex, sources);
            Assert.Contains(ScannerPaperSource.FeederSimplex, sources);
        }

        [Fact]
        public void ScannerPaperSource_CanBeUsedInDictionary()
        {
            // Arrange
            var sourceDescriptions = new Dictionary<ScannerPaperSource, string>
            {
                { ScannerPaperSource.Auto, "Automatic paper source selection" },
                { ScannerPaperSource.Flatbed, "Flatbed scanner" },
                { ScannerPaperSource.FeederDuplex, "Duplex document feeder" },
                { ScannerPaperSource.FeederSimplex, "Simplex document feeder" }
            };

            // Act & Assert
            Assert.Equal(4, sourceDescriptions.Count);
            Assert.Equal("Automatic paper source selection", sourceDescriptions[ScannerPaperSource.Auto]);
            Assert.Equal("Flatbed scanner", sourceDescriptions[ScannerPaperSource.Flatbed]);
            Assert.Equal("Duplex document feeder", sourceDescriptions[ScannerPaperSource.FeederDuplex]);
            Assert.Equal("Simplex document feeder", sourceDescriptions[ScannerPaperSource.FeederSimplex]);
        }

        [Fact]
        public void ScannerPaperSource_CanBeSerialized()
        {
            // Arrange
            var paperSource = ScannerPaperSource.FeederSimplex;

            // Act
            var serialized = paperSource.ToString();

            // Assert
            Assert.Equal("FeederSimplex", serialized);
        }

        [Fact]
        public void ScannerPaperSource_CanBeDeserialized()
        {
            // Arrange
            var serialized = "Flatbed";

            // Act
            var deserialized = Enum.Parse<ScannerPaperSource>(serialized);

            // Assert
            Assert.Equal(ScannerPaperSource.Flatbed, deserialized);
        }

        [Theory]
        [InlineData(ScannerPaperSource.Auto, "Auto")]
        [InlineData(ScannerPaperSource.Flatbed, "Flatbed")]
        [InlineData(ScannerPaperSource.FeederDuplex, "FeederDuplex")]
        [InlineData(ScannerPaperSource.FeederSimplex, "FeederSimplex")]
        public void ScannerPaperSource_ToString_ReturnsExpectedValue(ScannerPaperSource source, string expected)
        {
            // Act
            var result = source.ToString();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Auto", ScannerPaperSource.Auto)]
        [InlineData("Flatbed", ScannerPaperSource.Flatbed)]
        [InlineData("FeederDuplex", ScannerPaperSource.FeederDuplex)]
        [InlineData("FeederSimplex", ScannerPaperSource.FeederSimplex)]
        public void ScannerPaperSource_Parse_ReturnsExpectedValue(string value, ScannerPaperSource expected)
        {
            // Act
            var result = Enum.Parse<ScannerPaperSource>(value);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ScannerPaperSource_CanBeUsedInLinq()
        {
            // Arrange
            var sources = Enum.GetValues<ScannerPaperSource>();

            // Act
            var autoSources = sources.Where(s => s.ToString().Contains("Auto")).ToList();
            var feederSources = sources.Where(s => s.ToString().Contains("Feeder")).ToList();

            // Assert
            Assert.Single(autoSources);
            Assert.Equal(ScannerPaperSource.Auto, autoSources[0]);
            Assert.Equal(2, feederSources.Count);
            Assert.Contains(ScannerPaperSource.FeederDuplex, feederSources);
            Assert.Contains(ScannerPaperSource.FeederSimplex, feederSources);
        }
    }
} 
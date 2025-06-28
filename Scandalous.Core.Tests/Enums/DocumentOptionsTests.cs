using Scandalous.Core.Enums;
using Xunit;

namespace Scandalous.Core.Tests.Enums
{
    public class DocumentOptionsTests
    {
        [Fact]
        public void DocumentOptions_ContainsExpectedValues()
        {
            // Arrange & Act
            var values = Enum.GetValues<DocumentOptions>();

            // Assert
            Assert.Equal(2, values.Length);
            Assert.Contains(DocumentOptions.Individual, values);
            Assert.Contains(DocumentOptions.Combined, values);
        }

        [Fact]
        public void DocumentOptions_ValuesHaveCorrectUnderlyingIntegers()
        {
            // Assert
            Assert.Equal(0, (int)DocumentOptions.Individual);
            Assert.Equal(1, (int)DocumentOptions.Combined);
        }

        [Fact]
        public void DocumentOptions_CanBeParsedFromString()
        {
            // Arrange & Act & Assert
            Assert.Equal(DocumentOptions.Individual, Enum.Parse<DocumentOptions>("Individual"));
            Assert.Equal(DocumentOptions.Combined, Enum.Parse<DocumentOptions>("Combined"));
        }

        [Fact]
        public void DocumentOptions_CanBeParsedFromInteger()
        {
            // Arrange & Act & Assert
            Assert.Equal(DocumentOptions.Individual, (DocumentOptions)0);
            Assert.Equal(DocumentOptions.Combined, (DocumentOptions)1);
        }

        [Fact]
        public void DocumentOptions_ToString_ReturnsCorrectValue()
        {
            // Arrange & Act & Assert
            Assert.Equal("Individual", DocumentOptions.Individual.ToString());
            Assert.Equal("Combined", DocumentOptions.Combined.ToString());
        }

        [Fact]
        public void DocumentOptions_CanBeUsedInSwitchExpression()
        {
            // Arrange
            var documentOption = DocumentOptions.Combined;

            // Act
            var result = documentOption switch
            {
                DocumentOptions.Individual => "individual",
                DocumentOptions.Combined => "combined",
                _ => "unknown"
            };

            // Assert
            Assert.Equal("combined", result);
        }

        [Fact]
        public void DocumentOptions_CanBeUsedInSwitchStatement()
        {
            // Arrange
            var documentOption = DocumentOptions.Individual;
            var result = "";

            // Act
            switch (documentOption)
            {
                case DocumentOptions.Individual:
                    result = "individual";
                    break;
                case DocumentOptions.Combined:
                    result = "combined";
                    break;
                default:
                    result = "unknown";
                    break;
            }

            // Assert
            Assert.Equal("individual", result);
        }

        [Fact]
        public void DocumentOptions_CanBeCompared()
        {
            // Arrange
            var individual = DocumentOptions.Individual;
            var combined = DocumentOptions.Combined;

            // Act & Assert
            Assert.True(individual < combined);
            Assert.True(combined > individual);
            Assert.False(individual > combined);
            Assert.False(combined < individual);
        }

        [Fact]
        public void DocumentOptions_CanBeUsedInCollections()
        {
            // Arrange
            var options = new List<DocumentOptions>
            {
                DocumentOptions.Individual,
                DocumentOptions.Combined
            };

            // Act & Assert
            Assert.Equal(2, options.Count);
            Assert.Contains(DocumentOptions.Individual, options);
            Assert.Contains(DocumentOptions.Combined, options);
        }

        [Fact]
        public void DocumentOptions_CanBeUsedInDictionary()
        {
            // Arrange
            var optionDescriptions = new Dictionary<DocumentOptions, string>
            {
                { DocumentOptions.Individual, "Save each page as a separate file" },
                { DocumentOptions.Combined, "Save all pages in a single file" }
            };

            // Act & Assert
            Assert.Equal(2, optionDescriptions.Count);
            Assert.Equal("Save each page as a separate file", optionDescriptions[DocumentOptions.Individual]);
            Assert.Equal("Save all pages in a single file", optionDescriptions[DocumentOptions.Combined]);
        }

        [Fact]
        public void DocumentOptions_CanBeSerialized()
        {
            // Arrange
            var documentOption = DocumentOptions.Individual;

            // Act
            var serialized = documentOption.ToString();

            // Assert
            Assert.Equal("Individual", serialized);
        }

        [Fact]
        public void DocumentOptions_CanBeDeserialized()
        {
            // Arrange
            var serialized = "Combined";

            // Act
            var deserialized = Enum.Parse<DocumentOptions>(serialized);

            // Assert
            Assert.Equal(DocumentOptions.Combined, deserialized);
        }

        [Theory]
        [InlineData(DocumentOptions.Individual, "Individual")]
        [InlineData(DocumentOptions.Combined, "Combined")]
        public void DocumentOptions_ToString_ReturnsExpectedValue(DocumentOptions option, string expected)
        {
            // Act
            var result = option.ToString();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Individual", DocumentOptions.Individual)]
        [InlineData("Combined", DocumentOptions.Combined)]
        public void DocumentOptions_Parse_ReturnsExpectedValue(string value, DocumentOptions expected)
        {
            // Act
            var result = Enum.Parse<DocumentOptions>(value);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DocumentOptions_CanBeUsedInLinq()
        {
            // Arrange
            var options = Enum.GetValues<DocumentOptions>();

            // Act
            var individualOptions = options.Where(o => o == DocumentOptions.Individual).ToList();
            var combinedOptions = options.Where(o => o == DocumentOptions.Combined).ToList();

            // Assert
            Assert.Single(individualOptions);
            Assert.Equal(DocumentOptions.Individual, individualOptions[0]);
            Assert.Single(combinedOptions);
            Assert.Equal(DocumentOptions.Combined, combinedOptions[0]);
        }

        [Fact]
        public void DocumentOptions_CanBeUsedInConditionalLogic()
        {
            // Arrange
            var individual = DocumentOptions.Individual;
            var combined = DocumentOptions.Combined;

            // Act & Assert
            Assert.True(individual == DocumentOptions.Individual);
            Assert.True(combined == DocumentOptions.Combined);
            Assert.False(individual == DocumentOptions.Combined);
            Assert.False(combined == DocumentOptions.Individual);
        }

        [Fact]
        public void DocumentOptions_CanBeUsedInBooleanExpressions()
        {
            // Arrange
            var individual = DocumentOptions.Individual;
            var combined = DocumentOptions.Combined;

            // Act & Assert
            Assert.True(individual != combined);
            Assert.False(individual == combined);
            Assert.True(individual == DocumentOptions.Individual);
            Assert.True(combined == DocumentOptions.Combined);
        }

        [Fact]
        public void DocumentOptions_CanBeUsedInArray()
        {
            // Arrange
            var options = new DocumentOptions[]
            {
                DocumentOptions.Individual,
                DocumentOptions.Combined
            };

            // Act & Assert
            Assert.Equal(2, options.Length);
            Assert.Equal(DocumentOptions.Individual, options[0]);
            Assert.Equal(DocumentOptions.Combined, options[1]);
        }

        [Fact]
        public void DocumentOptions_CanBeUsedInHashSet()
        {
            // Arrange
            var options = new HashSet<DocumentOptions>
            {
                DocumentOptions.Individual,
                DocumentOptions.Combined
            };

            // Act & Assert
            Assert.Equal(2, options.Count);
            Assert.Contains(DocumentOptions.Individual, options);
            Assert.Contains(DocumentOptions.Combined, options);
        }
    }
} 
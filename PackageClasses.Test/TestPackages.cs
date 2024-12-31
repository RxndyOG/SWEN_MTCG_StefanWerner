using packageClasses;

namespace PackageClasses.Test
{
    public class TestPackages
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void testCreatePackage_scenarioWithCorrectInput()
        {
            // ARRANGE
            Packages packages = new Packages();
            var cardsData = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object> { { "Id", "1" }, { "Name", "Fireball" }, { "Damage", "50" } },
            new Dictionary<string, object> { { "Id", "2" }, { "Name", "Icebolt" }, { "Damage", "30" } },
            new Dictionary<string, object> { { "Id", "3" }, { "Name", "Earthquake" }, { "Damage", "70" } },
            new Dictionary<string, object> { { "Id", "4" }, { "Name", "Thunderstorm" }, { "Damage", "90" } },
            new Dictionary<string, object> { { "Id", "5" }, { "Name", "Windblast" }, { "Damage", "40" } },
        };

            // ACT
            var (package, status) = packages.createPackage("Bearer admin-mtcgToken", cardsData);

            // ASSERT
            Assert.IsNotNull(package);
            Assert.AreEqual(0, status);
            Assert.AreEqual(5, package.getPack.Count);
        }

    }
}
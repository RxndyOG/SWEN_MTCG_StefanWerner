namespace CardsClasses.Test
{
    public class TestCards
    {
        [SetUp]
        public void Setup()
        {
        }

        // testet ob die Karte correct erstellt wird
        // should Succede
        [Test]
        public void testCreateCard_scenarioWithCorrectInput()
        {
            //ARRANGE
            string IDTested = "10cardtest";
            string NameTested = "testedCard";
            float DamageTested = 10.0F;

            //ACT
            Cards testedCard = new Cards().createCard(IDTested,NameTested,DamageTested);

            //ASSERT
            Assert.AreEqual(IDTested, testedCard.SetGetID);
            Assert.AreEqual(NameTested, testedCard.SetGetName);
            Assert.AreEqual(DamageTested, testedCard.SetGetDamage);
        }

        // testet ob die karte ein spell ist
        // should succede
        [Test]
        public void testParseCardType_scenarioSpell()
        {
            // ARRANGE
            var card = new Cards();
            var method = typeof(Cards).GetMethod("parseCardType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // ACT
            method.Invoke(card, new object[] { "FireSpell" });

            // ASSERT
            Assert.AreEqual(1, card.SetGetCardType);
        }

        // testet ob die karte ein Monster ist
        // should succede
        [Test]
        public void testParseCardType_scenarioMonster()
        {
            // ARRANGE
            var card = new Cards();
            var method = typeof(Cards).GetMethod("parseCardType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // ACT
            method.Invoke(card, new object[] { "Dragon" });

            // ASSERT
            Assert.AreEqual(0, card.SetGetCardType);
        }

        // testet das element der Karte mit Spell ist
        // should succede
        [Test]
        public void testParseName_scenarioFire()
        {
            // ARRANGE
            var card = new Cards();
            var method = typeof(Cards).GetMethod("parseName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // ACT
            method.Invoke(card, new object[] { "FireSpell" });

            // ASSERT
            Assert.AreEqual("Fire", card.SetGetElement);
        }

        // testet das element der Karte mit Monster ist
        // should succede
        [Test]
        public void testParseName_scenarioWater()
        {
            // ARRANGE
            var card = new Cards();
            var method = typeof(Cards).GetMethod("parseName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // ACT
            method.Invoke(card, new object[] { "WaterDragon" });

            // ASSERT
            Assert.AreEqual("Water", card.SetGetElement);
        }

        // testet das element der Karte mit Spell ist
        // should succede
        [Test]
        public void testParseName_scenarioNormal()
        {
            // ARRANGE
            var card = new Cards();
            var method = typeof(Cards).GetMethod("parseName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // ACT
            method.Invoke(card, new object[] { "Dragon" });

            // ASSERT
            Assert.AreEqual("Normal", card.SetGetElement);
        }

    }
}
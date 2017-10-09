using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaLinks;

namespace MediaLinksTests
{
    [TestClass]
    public class MediaLinkParserTests
    {
        [DataTestMethod]
        [DataRow(new string[0], "")]
        [DataRow(new[] { "Fargo" }, "Fargo")]
        [DataRow(new[] { "Malcolm", "mittendrin" }, "Malcolm mittendrin")]
        [DataRow(new[] { "Game", "of", "Thrones" }, "Game of Thrones")]
        [DataRow(new[] { "House", "of", "Cards" }, "House of Cards (US)")]
        [DataRow(new[] { "Fringe", "–", "Grenzfälle", "des", "FBI" }, "Fringe - Grenzfälle des FBI")]
        [DataRow(new[] { "Ich,", "beide", "%26", "sie" }, "Ich, beide & sie")]
        [DataRow(new[] { "2", "Fast", "2", "Furious" }, "2 Fast 2 Furious (2003) [tt0322259].avi")]
        [DataRow(new[] { "96", "Hours", "–", "Taken", "2" }, "96 Hours - Taken 2 (Extended Cut) (2012) [tt1397280].mp4")]
        [DataRow(new[] { "Straight", "Outta", "Compton" }, "Straight Outta Compton (2015) (Directors Cut) (Remux) [tt1398426].mkv")]
        [DataRow(new[] { "Birdman", "oder" }, "Birdman oder (Die unverhoffte Macht der Ahnungslosigkeit) (2014) [tt2562232].iso")]
        [DataRow(new[] { "Der", "Kautions-Cop" }, "Der Kautions-Cop (2010) [tt1038919].mpg")]
        [DataRow(new[] { "Ghost", "in", "the", "Shell", "2", "–", "Innocence" }, "Ghost in the Shell 2 - Innocence (Remux) (2004) [tt0347246].img")]
        [DataRow(new[] { "Black", "Sabbath", "End", "Of", "The", "Beginning" }, "01_01_Black Sabbath_End Of The Beginning.flac")]
        [DataRow(new[] { "Deadmau5", "I", "Said" }, "01_06_Deadmau5_I Said (Michael Woods Remix).flac")]
        [DataRow(new[] { "Daft", "Punk", "The", "Grid" }, "Daft Punk - The Grid (Remixed By The Crystal Method).mp3")]
        [DataRow(new[] { "Flogging", "Molly", "Paddy's", "Lament" }, "01_09_Flogging Molly_(No More) Paddy's Lament.ogg")]
        [DataRow(new[] { "Redman", "%26", "Gorillaz", "Gorillaz", "On", "My", "Mind" }, "Redman & Gorillaz - Gorillaz On My Mind.mpc")]
        [DataRow(new[] { "Gorillaz", "M1", "A1", "Slow", "Country", "Clint", "Eastwood" }, "01_12_Gorillaz_M1 A1 (Lil' Dub Chefin') + Slow Country (More Rubba Dub) + Clint Eastwood (More Peanuts).flac")]
        [DataRow(new[] { "2010", "TRON", "Legacy" }, "2010_TRON Legacy (Special Edtion)")]
        public void Parse_WithValidString_ParsesWords(string[] expected, string filename)
        {
            // arrange
            const Key key = default(Key);
            var parser = new MediaLinkParser();
            
            // act
            parser.Parse(key, filename);
            
            // assert
            var actual = parser.Words;
            CollectionAssert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(null, "")]
        [DataRow(null, "Birdman oder (Die unverhoffte Macht der Ahnungslosigkeit) (2014).mp4")]
        [DataRow("tt1038919", "Der Kautions-Cop (2010) [tt1038919].avi")]
        public void Parse_WithValidString_ParsesImdbId(string expected, string filename)
        {
            // arrange
            const Key key = default(Key);
            var parser = new MediaLinkParser();

            // act
            parser.Parse(key, filename);

            // assert
            var actual = parser.ImdbId;
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow((Key) int.MaxValue, "", null)]
        [DataRow(Key.GoogleSearchEpisodesFeelingLucky, "House of Cards (US)", "http://www.google.de/search?q=House+of+Cards+Episoden&btnI")]
        public void Parse_WithValidString_ParsesUrl(Key key, string filename, string expected)
        {
            // arrange
            var parser = new MediaLinkParser();

            // act
            parser.Parse(key, filename);

            // assert
            var actual = parser.Url;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Parse_WithInvalidKey_ThrowsArgumentOutOfRangeException()
        {
            // arrange / act
            const Key key = (Key)int.MaxValue;
            const string filename = "Fargo";
            var parser = new MediaLinkParser();

            // assert
            parser.Parse(key, filename);
        }
    }
}

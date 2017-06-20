using System;
using Ale2Project.Model;
using Ale2Project.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ale2ProjectUnitTest
{
    [TestClass]
    public class Assignment6Test
    {
        [TestMethod]
        public void IsAcceptedStringByPda_Test()
        {
            //arrange
            string empty = "ε";

            LanguageCheckService languageCheckService = new LanguageCheckService();

            AutomatonModel pda = new AutomatonModel();

            StateModel stateS = new StateModel()
            {
                IsFinal = true,
                IsInitial = true,
                Name = "S"
            };
            pda.States.Add(stateS);

            TransitionModel transition1 = new TransitionModel() { BeginState = stateS, EndState = stateS, Value = "a", PopStack = empty, PushStack = "x" };
            TransitionModel transition2 = new TransitionModel() { BeginState = stateS, EndState = stateS, Value = "b", PopStack = empty, PushStack = "y" };
            TransitionModel transition3 = new TransitionModel() { BeginState = stateS, EndState = stateS, Value = "c", PopStack = "x", PushStack = empty };
            TransitionModel transition4 = new TransitionModel() { BeginState = stateS, EndState = stateS, Value = "d", PopStack = "y", PushStack = empty };
            TransitionModel transition5 = new TransitionModel() { BeginState = stateS, EndState = stateS, Value = empty, PopStack = "x", PushStack = empty };
            TransitionModel transition6 = new TransitionModel() { BeginState = stateS, EndState = stateS, Value = empty, PopStack = "y", PushStack = empty };

            pda.Transitions.Add(transition1);
            pda.Transitions.Add(transition2);
            pda.Transitions.Add(transition3);
            pda.Transitions.Add(transition4);
            pda.Transitions.Add(transition5);
            pda.Transitions.Add(transition6);

            pda.AccecptedStack.Add("x");
            pda.AccecptedStack.Add("y");

            pda.Alphabet.Add('a');
            pda.Alphabet.Add('b');
            pda.Alphabet.Add('c');
            pda.Alphabet.Add('d');

            var word1 = "";
            var word2 = "a";
            var word3 = "ac";
            var word4 = "b";
            var word5 = "bd";
            var word6 = "aacc";
            var word7 = "bbdd";
            var word8 = "bc";
            var word9 = "c";
            var word10 = "d";

            var expectedWord1 = false;
            var expectedWord2 = true;
            var expectedWord3 = true;
            var expectedWord4 = true;
            var expectedWord5 = true;
            var expectedWord6 = true;
            var expectedWord7 = true;
            var expectedWord8 = false;
            var expectedWord9 = false;
            var expectedWord10 = false;

            //act
            var actualWord1 = languageCheckService.IsAcceptedStringByPda(word1, pda);
            var actualWord2 = languageCheckService.IsAcceptedStringByPda(word2, pda);
            var actualWord3 = languageCheckService.IsAcceptedStringByPda(word3, pda);
            var actualWord4 = languageCheckService.IsAcceptedStringByPda(word4, pda);
            var actualWord5 = languageCheckService.IsAcceptedStringByPda(word5, pda);
            var actualWord6 = languageCheckService.IsAcceptedStringByPda(word6, pda);
            var actualWord7 = languageCheckService.IsAcceptedStringByPda(word7, pda);
            var actualWord8 = languageCheckService.IsAcceptedStringByPda(word8, pda);
            var actualWord9 = languageCheckService.IsAcceptedStringByPda(word9, pda);
            var actualWord10 = languageCheckService.IsAcceptedStringByPda(word10, pda);

            //assert
            Assert.AreEqual(expectedWord1, actualWord1);
            Assert.AreEqual(expectedWord2, actualWord2);
            Assert.AreEqual(expectedWord3, actualWord3);
            Assert.AreEqual(expectedWord4, actualWord4);
            Assert.AreEqual(expectedWord5, actualWord5);
            Assert.AreEqual(expectedWord6, actualWord6);
            Assert.AreEqual(expectedWord7, actualWord7);
            Assert.AreEqual(expectedWord8, actualWord8);
            Assert.AreEqual(expectedWord9, actualWord9);
            Assert.AreEqual(expectedWord10, actualWord10);
        }
    }
}

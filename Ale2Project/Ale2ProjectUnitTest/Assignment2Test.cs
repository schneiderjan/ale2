using System;
using System.Collections.Generic;
using Ale2Project.Model;
using Ale2Project.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ale2ProjectUnitTest
{
    [TestClass]
    public class Assignment2Test
    {
        [TestMethod]
        public void IsAcceptedString_Test()
        {
            //arrange
            LanguageCheckService languageCheckService = new LanguageCheckService();

            AutomatonModel automaton = new AutomatonModel() { IsDfaInFile = false };

            var stateZ = new StateModel() { IsFinal = false, IsInitial = true, Name = "z" };
            var stateA = new StateModel() { IsFinal = false, IsInitial = false, Name = "a" };
            var stateB = new StateModel() { IsFinal = true, IsInitial = false, Name = "b" };

            List<StateModel> states = new List<StateModel> { stateB, stateA, stateZ };
            List<TransitionModel> transitions = new List<TransitionModel>()
            {
                new TransitionModel() {BeginState = stateZ, EndState = stateA, Value = "a"},
                new TransitionModel() {BeginState = stateZ, EndState = stateZ, Value = "b"},

                new TransitionModel() {BeginState = stateA, EndState = stateZ, Value = "b"},
                new TransitionModel() {BeginState = stateA, EndState = stateB, Value = "a"},

                new TransitionModel() {BeginState = stateB, EndState = stateB, Value = "b"},
                new TransitionModel() {BeginState = stateB, EndState = stateZ, Value = "a"},
            };
            var alphabet = new List<char>() { 'a', 'b' };

            automaton.States = states;
            automaton.Transitions = transitions;
            automaton.Alphabet = alphabet;

            //accepted
            var word1 = "aab";
            var word2 = "baab";
            var word3 = "abaa";
            var word4 = "abaab";

            //not accepted
            var word5 = "abb";
            var word6 = "ba";
            var word7 = "aaa";

            var expected_word1 = true;
            var expected_word2 = true;
            var expected_word3 = true;
            var expected_word4 = true;

            var expected_word5 = false;
            var expected_word6 = false;
            var expected_word7 = false;

            //act

            //accepted
            var actual_word1 = languageCheckService.IsAcceptedString(word1, automaton);
            var actual_word2 = languageCheckService.IsAcceptedString(word2, automaton);
            var actual_word3 = languageCheckService.IsAcceptedString(word3, automaton);
            var actual_word4 = languageCheckService.IsAcceptedString(word4, automaton);

            //not accepted
            var actual_word5 = languageCheckService.IsAcceptedString(word5, automaton);
            var actual_word6 = languageCheckService.IsAcceptedString(word6, automaton);
            var actual_word7 = languageCheckService.IsAcceptedString(word7, automaton);

            //assert
            Assert.AreEqual(expected_word1, actual_word1);
            Assert.AreEqual(expected_word2, actual_word2);
            Assert.AreEqual(expected_word3, actual_word3);
            Assert.AreEqual(expected_word4, actual_word4);

            Assert.AreEqual(expected_word5, actual_word5);
            Assert.AreEqual(expected_word6, actual_word6);
            Assert.AreEqual(expected_word7, actual_word7);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ale2Project;
using Ale2Project.Model;
using Ale2Project.Service;
using Ale2Project.ViewModel;

namespace Ale2ProjectUnitTest
{
    [TestClass]
    public class Assignment1Test
    {
        [TestMethod]
        public void IsAutomatonDfaWithDfa_Test()
        {
            // arrange  
            DfaService dfaService = new DfaService();

            AutomatonModel automaton = new AutomatonModel() { IsDfaInFile = true };

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

            var expected = true;

            // act
            var actual = dfaService.IsAutomatonDfa(automaton);

            // assert  
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsAutomatonDfaWithNonDfa_Test()
        {
            // arrange  
            DfaService dfaService = new DfaService();

            AutomatonModel automaton = new AutomatonModel() { IsDfaInFile = true };

            var stateZ = new StateModel() { IsFinal = false, IsInitial = true, Name = "z" };
            var stateA = new StateModel() { IsFinal = false, IsInitial = false, Name = "a" };
            var stateB = new StateModel() { IsFinal = true, IsInitial = false, Name = "b" };

            List<StateModel> states = new List<StateModel> { stateB, stateA, stateZ };
            List<TransitionModel> transitions = new List<TransitionModel>()
            {
                new TransitionModel() {BeginState = stateZ, EndState = stateA, Value = "a"},
                new TransitionModel() {BeginState = stateZ, EndState = stateZ, Value = "b"},
                new TransitionModel() {BeginState = stateZ, EndState = stateB, Value = "b"},

                new TransitionModel() {BeginState = stateA, EndState = stateZ, Value = "b"},
                new TransitionModel() {BeginState = stateA, EndState = stateB, Value = "a"},

                new TransitionModel() {BeginState = stateB, EndState = stateB, Value = "b"},
                new TransitionModel() {BeginState = stateB, EndState = stateZ, Value = "a"},
            };
            var alphabet = new List<char>() { 'a', 'b' };

            automaton.States = states;
            automaton.Transitions = transitions;
            automaton.Alphabet = alphabet;

            var expected = false;

            // act
            var actual = dfaService.IsAutomatonDfa(automaton);

            // assert  
            Assert.AreEqual(expected, actual);
        }
    }
}

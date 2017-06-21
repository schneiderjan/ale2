using System;
using System.Linq;
using Ale2Project.Model;
using Ale2Project.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ale2ProjectUnitTest
{
    [TestClass]
    public class Assignment5Test
    {
        [TestMethod]
        public void ConvertNdfaToDfa_Test1()
        {
            //arrange
            DfaService dfaService = new DfaService();
            
            AutomatonModel ndfa = new AutomatonModel();

            StateModel stateA = new StateModel() {IsFinal = true, IsInitial = true, Name = "A"};
            StateModel stateB = new StateModel() {IsFinal = false, IsInitial = false, Name = "B"};
            StateModel stateC = new StateModel() {IsFinal = false, IsInitial = false, Name = "C"};
            StateModel stateD = new StateModel() {IsFinal = false, IsInitial = false, Name = "D"};
            ndfa.States.Add(stateA);
            ndfa.States.Add(stateB);
            ndfa.States.Add(stateC);
            ndfa.States.Add(stateD);

            TransitionModel trans1 = new TransitionModel() {BeginState = stateA, EndState = stateB, Value = "a"};
            TransitionModel trans2 = new TransitionModel() {BeginState = stateA, EndState = stateC, Value = "a"};
            TransitionModel trans3 = new TransitionModel() {BeginState = stateB, EndState = stateA, Value = "b"};
            TransitionModel trans4 = new TransitionModel() {BeginState = stateC, EndState = stateB, Value = "b"};
            TransitionModel trans5 = new TransitionModel() {BeginState = stateC, EndState = stateD, Value = "b"};
            TransitionModel trans6 = new TransitionModel() {BeginState = stateD, EndState = stateB, Value = "b"};
            ndfa.Transitions.Add(trans1);
            ndfa.Transitions.Add(trans2);
            ndfa.Transitions.Add(trans3);
            ndfa.Transitions.Add(trans4);
            ndfa.Transitions.Add(trans5);
            ndfa.Transitions.Add(trans6);
            
            ndfa.Alphabet.Add('a');
            ndfa.Alphabet.Add('b');

            int expected_nr_transitions = 6;
            int expected_transitions_with_a = 3;
            int expected_transitions_with_b = 3;
            int expected_nr_states = 4;

            //act
            var dfa = dfaService.ConvertNdfaToDfa(ndfa);
            
            int actual_nr_transitions = dfa.Transitions.Count;
            int actual_transitions_with_a = dfa.Transitions.Where(t => t.Value == "a").ToList().Count;
            int actual_transitions_with_b = dfa.Transitions.Where(t => t.Value == "b").ToList().Count;
            int actual_nr_states = dfa.States.Count;
            
            //assert
            Assert.AreEqual(expected_nr_states, actual_nr_states);
            Assert.AreEqual(expected_nr_transitions, actual_nr_transitions);
            Assert.AreEqual(expected_transitions_with_a, actual_transitions_with_a);
            Assert.AreEqual(expected_transitions_with_b, actual_transitions_with_b);
        }


        [TestMethod]
        public void ConvertNdfaToDfa_Test2()
        {
            //arrange
            DfaService dfaService = new DfaService();

            AutomatonModel ndfa = new AutomatonModel();

            StateModel state1 = new StateModel() { IsFinal = false, IsInitial = true, Name = "1" };
            StateModel state2 = new StateModel() { IsFinal = false, IsInitial = false, Name = "2" };
            StateModel state3 = new StateModel() { IsFinal = true, IsInitial = false, Name = "3" };
            StateModel state4 = new StateModel() { IsFinal = false, IsInitial = false, Name = "4" };
            ndfa.States.Add(state1);
            ndfa.States.Add(state2);
            ndfa.States.Add(state3);
            ndfa.States.Add(state4);

            TransitionModel trans1 = new TransitionModel() { BeginState = state1, EndState = state2, Value = "a" };
            TransitionModel trans2 = new TransitionModel() { BeginState = state1, EndState = state4, Value = "c" };
            TransitionModel trans3 = new TransitionModel() { BeginState = state2, EndState = state1, Value = "ε" };
            TransitionModel trans4 = new TransitionModel() { BeginState = state2, EndState = state3, Value = "b" };
            TransitionModel trans5 = new TransitionModel() { BeginState = state3, EndState = state2, Value = "a" };
            TransitionModel trans6 = new TransitionModel() { BeginState = state4, EndState = state3, Value = "ε" };
            TransitionModel trans7 = new TransitionModel() { BeginState = state4, EndState = state3, Value = "c" };
            ndfa.Transitions.Add(trans1);
            ndfa.Transitions.Add(trans2);
            ndfa.Transitions.Add(trans3);
            ndfa.Transitions.Add(trans4);
            ndfa.Transitions.Add(trans5);
            ndfa.Transitions.Add(trans6);
            ndfa.Transitions.Add(trans7);

            ndfa.Alphabet.Add('a');
            ndfa.Alphabet.Add('b');
            ndfa.Alphabet.Add('c');

            int expected_nr_transitions = 8;
            int expected_transitions_with_a = 4;
            int expected_transitions_with_b = 1;
            int expected_transitions_with_c = 3;
            int expected_nr_states = 4;

            //act 
            var dfa = dfaService.ConvertNdfaToDfa(ndfa);

            int actual_nr_transitions = dfa.Transitions.Count;
            int actual_transitions_with_a = dfa.Transitions.Where(t => t.Value == "a").ToList().Count;
            int actual_transitions_with_b = dfa.Transitions.Where(t => t.Value == "b").ToList().Count;
            int actual_transitions_with_c = dfa.Transitions.Where(t => t.Value == "c").ToList().Count;
            int actual_nr_states = dfa.States.Count;

            //assert
            Assert.AreEqual(expected_nr_states, actual_nr_states);
            Assert.AreEqual(expected_nr_transitions, actual_nr_transitions);
            Assert.AreEqual(expected_transitions_with_a, actual_transitions_with_a);
            Assert.AreEqual(expected_transitions_with_b, actual_transitions_with_b);
            Assert.AreEqual(expected_transitions_with_c, actual_transitions_with_c);
        }
    }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.OffspringSelectionEvolutionStrategy {
  /// <summary>
  /// An operator which represents the main loop of an evolution strategy (OffspringSelectionEvolutionStrategy).
  /// </summary>
  [Item("OffspringSelectionEvolutionStrategyMainLoop", "An operator which represents the main loop of an evolution strategy with offspring selection (OffspringSelectionEvolutionStrategy).")]
  [StorableClass]
  public sealed class OffspringSelectionEvolutionStrategyMainLoop : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public ValueLookupParameter<IntValue> PopulationSizeParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    public ValueLookupParameter<IntValue> ParentsPerChildParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["ParentsPerChild"]; }
    }
    public ValueLookupParameter<BoolValue> PlusSelectionParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["PlusSelection"]; }
    }
    public IValueLookupParameter<BoolValue> ReevaluateElitesParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["ReevaluateElites"]; }
    }
    public ValueLookupParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumGenerations"]; }
    }
    public ValueLookupParameter<IOperator> MutatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Mutator"]; }
    }
    public ValueLookupParameter<IOperator> RecombinatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Recombinator"]; }
    }
    public ValueLookupParameter<IOperator> EvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Evaluator"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    public ValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    public LookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (LookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    private ValueLookupParameter<IOperator> StrategyParameterManipulatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["StrategyParameterManipulator"]; }
    }
    private ValueLookupParameter<IOperator> StrategyParameterCrossoverParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["StrategyParameterCrossover"]; }
    }

    public LookupParameter<DoubleValue> CurrentSuccessRatioParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["CurrentSuccessRatio"]; }
    }
    public LookupParameter<DoubleValue> SelectionPressureParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["SelectionPressure"]; }
    }
    public ValueLookupParameter<DoubleValue> SuccessRatioParameter {
       get { return (ValueLookupParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    public ValueLookupParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    public ValueLookupParameter<IntValue> MaximumEvaluatedSolutionsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumEvaluatedSolutions"]; }
    }
    public ValueLookupParameter<IntValue> SelectedParentsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["SelectedParents"]; }
    }
    public LookupParameter<DoubleValue> ComparisonFactorParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
    }

    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private OffspringSelectionEvolutionStrategyMainLoop(bool deserializing) : base(deserializing) { }
    private OffspringSelectionEvolutionStrategyMainLoop(OffspringSelectionEvolutionStrategyMainLoop original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OffspringSelectionEvolutionStrategyMainLoop(this, cloner);
    }
    public OffspringSelectionEvolutionStrategyMainLoop()
      : base() {
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "µ (mu) - the size of the population."));
      Parameters.Add(new ValueLookupParameter<IntValue>("ParentsPerChild", "ρ (rho) - how many parents should be recombined."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("PlusSelection", "True for plus selection (elitist population), false for comma selection (non-elitist population)."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Recombinator", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze each generation."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of times solutions have been evaluated."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope which represents a population of solutions on which the OffspringSelectionEvolutionStrategy should be applied."));
      Parameters.Add(new ValueLookupParameter<IOperator>("StrategyParameterManipulator", "The operator to mutate the endogeneous strategy parameters."));
      Parameters.Add(new ValueLookupParameter<IOperator>("StrategyParameterCrossover", "The operator to cross the endogeneous strategy parameters."));
     
      Parameters.Add(new LookupParameter<DoubleValue>("CurrentSuccessRatio", "The current success ratio."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved."));
      Parameters.Add(new LookupParameter<DoubleValue>("SelectionPressure", "The actual selection pressure."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumEvaluatedSolutions", "The maximum number of evaluated solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SelectedParents", "How much parents should be selected each time the offspring selection step is performed until the population is filled. This parameter should be about the same or twice the size of PopulationSize for smaller problems, and less for large problems."));
      Parameters.Add(new LookupParameter<DoubleValue>("ComparisonFactor", "The comparison factor is used to determine whether the offspring should be compared to the better parent, the worse parent or a quality value linearly interpolated between them. It is in the range [0;1]."));

      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      Placeholder analyzer1 = new Placeholder();
      WithoutRepeatingBatchedRandomSelector selector = new WithoutRepeatingBatchedRandomSelector();
      SubScopesProcessor subScopesProcessor1 = new SubScopesProcessor();
      Comparator useRecombinationComparator = new Comparator();
      ConditionalBranch useRecombinationBranch = new ConditionalBranch();
      ChildrenCreator childrenCreator = new ChildrenCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      Placeholder recombinator = new Placeholder();
      Placeholder strategyRecombinator = new Placeholder();
      Placeholder strategyMutator1 = new Placeholder();
      Placeholder mutator1 = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      UniformSubScopesProcessor uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      Placeholder strategyMutator2 = new Placeholder();
      Placeholder mutator2 = new Placeholder();
      UniformSubScopesProcessor uniformSubScopesProcessor3 = new UniformSubScopesProcessor();
      Placeholder evaluator = new Placeholder();
      SubScopesCounter subScopesCounter = new SubScopesCounter();
      ConditionalBranch plusOrCommaReplacementBranch = new ConditionalBranch();
      MergingReducer plusReplacement = new MergingReducer();
      RightReducer commaReplacement = new RightReducer();
      BestSelector bestSelector = new BestSelector();
      RightReducer rightReducer = new RightReducer();
      IntCounter intCounter = new IntCounter();
      Comparator maxGenerationsComparator = new Comparator();
      Placeholder analyzer2 = new Placeholder();
      ConditionalBranch conditionalBranchTerminate = new ConditionalBranch();
      ConditionalBranch reevaluateElitesBranch = new ConditionalBranch();
      SubScopesProcessor subScopesProcessor2 = new SubScopesProcessor();
      UniformSubScopesProcessor uniformSubScopesProcessor4 = new UniformSubScopesProcessor();
      Placeholder evaluator2 = new Placeholder();
      SubScopesCounter subScopesCounter2 = new SubScopesCounter();
      WeightedParentsQualityComparator parentsComparator = new WeightedParentsQualityComparator();
      SubScopesRemover subScopesRemover_afterCompare = new SubScopesRemover();
      EvolutionStrategyOffspringSelector offspringSelector = new EvolutionStrategyOffspringSelector();
      ChildrenCopyCreator childrenCopyCreator = new ChildrenCopyCreator();
      Comparator maxSelectionPressureComparator = new Comparator();
      ConditionalBranch conditionalBranchTerminateSelPressure = new ConditionalBranch();
      Comparator maxEvaluatedSolutionsComparator = new Comparator();
      ConditionalBranch conditionalBranchTerminateEvalSolutions = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0))); // Class OffspringSelectionEvolutionStrategy expects this to be called Generations
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("SelectionPressure", new DoubleValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("CurrentSuccessRatio", new DoubleValue(0)));

      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Selection Pressure", "Displays the rising selection pressure during a generation.", "SelectionPressure"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", "Indicates how many successful children were already found during a generation (relative to the population size).", "CurrentSuccessRatio"));
      resultsCollector1.CopyValue = new BoolValue(false);
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      selector.Name = "ES Random Selector";
      selector.RandomParameter.ActualName = RandomParameter.Name;
      selector.ParentsPerChildParameter.ActualName = ParentsPerChildParameter.Name;
      selector.ChildrenParameter.ActualName = SelectedParentsParameter.Name;

      useRecombinationComparator.Name = "ParentsPerChild > 1";
      useRecombinationComparator.LeftSideParameter.ActualName = ParentsPerChildParameter.Name;
      useRecombinationComparator.RightSideParameter.Value = new IntValue(1);
      useRecombinationComparator.Comparison = new Comparison(ComparisonType.Greater);
      useRecombinationComparator.ResultParameter.ActualName = "UseRecombination";

      useRecombinationBranch.Name = "Use Recombination?";
      useRecombinationBranch.ConditionParameter.ActualName = "UseRecombination";

      childrenCreator.ParentsPerChild = null;
      childrenCreator.ParentsPerChildParameter.ActualName = ParentsPerChildParameter.Name;

      recombinator.Name = "Recombinator (placeholder)";
      recombinator.OperatorParameter.ActualName = RecombinatorParameter.Name;

      strategyRecombinator.Name = "Strategy Parameter Recombinator (placeholder)";
      strategyRecombinator.OperatorParameter.ActualName = StrategyParameterCrossoverParameter.Name;

      strategyMutator1.Name = "Strategy Parameter Manipulator (placeholder)";
      strategyMutator1.OperatorParameter.ActualName = StrategyParameterManipulatorParameter.Name;

      mutator1.Name = "Mutator (placeholder)";
      mutator1.OperatorParameter.ActualName = MutatorParameter.Name;

      subScopesRemover.RemoveAllSubScopes = true;

      strategyMutator2.Name = "Strategy Parameter Manipulator (placeholder)";
      strategyMutator2.OperatorParameter.ActualName = StrategyParameterManipulatorParameter.Name;

      mutator2.Name = "Mutator (placeholder)";
      mutator2.OperatorParameter.ActualName = MutatorParameter.Name;

      uniformSubScopesProcessor3.Parallel.Value = true;

      evaluator.Name = "Evaluator (placeholder)";
      evaluator.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesCounter.Name = "Increment EvaluatedSolutions";
      subScopesCounter.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;

      plusOrCommaReplacementBranch.ConditionParameter.ActualName = PlusSelectionParameter.Name;

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = PopulationSizeParameter.Name;
      bestSelector.QualityParameter.ActualName = QualityParameter.Name;

      intCounter.Increment = new IntValue(1);
      intCounter.ValueParameter.ActualName = "Generations";

      maxGenerationsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maxGenerationsComparator.LeftSideParameter.ActualName = "Generations";
      maxGenerationsComparator.ResultParameter.ActualName = "Terminate";
      maxGenerationsComparator.RightSideParameter.ActualName = MaximumGenerationsParameter.Name;

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      conditionalBranchTerminate.ConditionParameter.ActualName = "Terminate";

      reevaluateElitesBranch.ConditionParameter.ActualName = "ReevaluateElites";
      reevaluateElitesBranch.Name = "Reevaluate elites ?";

      uniformSubScopesProcessor4.Parallel.Value = true;

      evaluator2.Name = "Evaluator (placeholder)";
      evaluator2.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesCounter2.Name = "Increment EvaluatedSolutions";
      subScopesCounter2.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;

      parentsComparator.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      parentsComparator.LeftSideParameter.ActualName = QualityParameter.Name;
      parentsComparator.RightSideParameter.ActualName = QualityParameter.Name;
      parentsComparator.MaximizationParameter.ActualName = MaximizationParameter.Name;
      parentsComparator.ResultParameter.ActualName = "SuccessfulOffspring";

      subScopesRemover_afterCompare.RemoveAllSubScopes = true;

      offspringSelector.CurrentSuccessRatioParameter.ActualName = CurrentSuccessRatioParameter.Name;
      offspringSelector.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      offspringSelector.SelectionPressureParameter.ActualName = SelectionPressureParameter.Name;
      offspringSelector.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      offspringSelector.OffspringPopulationParameter.ActualName = "OffspringPopulation";
      offspringSelector.OffspringPopulationWinnersParameter.ActualName = "OffspringPopulationWinners";
      offspringSelector.SuccessfulOffspringParameter.ActualName = "SuccessfulOffspring";
      offspringSelector.QualityParameter.ActualName = QualityParameter.Name;

      maxSelectionPressureComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maxSelectionPressureComparator.LeftSideParameter.ActualName = "SelectionPressure";
      maxSelectionPressureComparator.ResultParameter.ActualName = "TerminateSelectionPressure";
      maxSelectionPressureComparator.RightSideParameter.ActualName = MaximumSelectionPressureParameter.Name;

      conditionalBranchTerminateSelPressure.ConditionParameter.ActualName = "TerminateSelectionPressure";

      maxEvaluatedSolutionsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maxEvaluatedSolutionsComparator.LeftSideParameter.ActualName = "EvaluatedSolutions";
      maxEvaluatedSolutionsComparator.ResultParameter.ActualName = "TerminateEvaluatedSolutions";
      maxEvaluatedSolutionsComparator.RightSideParameter.ActualName = MaximumEvaluatedSolutionsParameter.Name;

      conditionalBranchTerminateEvalSolutions.ConditionParameter.ActualName = "TerminateEvaluatedSolutions";

      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = resultsCollector1;
      resultsCollector1.Successor = analyzer1;
      analyzer1.Successor = selector;
      selector.Successor = subScopesProcessor1;
      subScopesProcessor1.Operators.Add(new EmptyOperator());
      subScopesProcessor1.Operators.Add(useRecombinationComparator);

      subScopesProcessor1.Successor = offspringSelector;
      offspringSelector.OffspringCreator = selector;
      offspringSelector.Successor = plusOrCommaReplacementBranch;

      useRecombinationComparator.Successor = useRecombinationBranch;
      useRecombinationBranch.TrueBranch = childrenCreator;

      useRecombinationBranch.FalseBranch = childrenCopyCreator;
      childrenCopyCreator.Successor = uniformSubScopesProcessor2;

      useRecombinationBranch.Successor = uniformSubScopesProcessor3;
      childrenCreator.Successor = uniformSubScopesProcessor1;
      uniformSubScopesProcessor1.Operator = recombinator;
      uniformSubScopesProcessor1.Successor = null;
      recombinator.Successor = strategyRecombinator;
      strategyRecombinator.Successor = strategyMutator1;
      strategyMutator1.Successor = mutator1;

      mutator1.Successor = null;

      uniformSubScopesProcessor2.Operator = strategyMutator2;
      uniformSubScopesProcessor2.Successor = null;
      strategyMutator2.Successor = mutator2;
      mutator2.Successor = null;
      uniformSubScopesProcessor3.Operator = evaluator;
      uniformSubScopesProcessor3.Successor = subScopesCounter;

      evaluator.Successor = parentsComparator;
      parentsComparator.Successor = subScopesRemover_afterCompare;
      subScopesRemover_afterCompare.Successor = null;
      subScopesCounter.Successor = null;

      plusOrCommaReplacementBranch.TrueBranch = reevaluateElitesBranch;
      reevaluateElitesBranch.TrueBranch = subScopesProcessor2;
      reevaluateElitesBranch.FalseBranch = null;
      subScopesProcessor2.Operators.Add(uniformSubScopesProcessor4);
      subScopesProcessor2.Operators.Add(new EmptyOperator());
      uniformSubScopesProcessor4.Operator = evaluator2;
      uniformSubScopesProcessor4.Successor = subScopesCounter2;
      subScopesCounter2.Successor = null;
      reevaluateElitesBranch.Successor = plusReplacement;

      plusReplacement.Successor = bestSelector;
      bestSelector.Successor = rightReducer;

      plusOrCommaReplacementBranch.FalseBranch = commaReplacement;
      plusOrCommaReplacementBranch.Successor = intCounter;


      intCounter.Successor = maxGenerationsComparator; 
      maxGenerationsComparator.Successor = maxSelectionPressureComparator;
      maxSelectionPressureComparator.Successor = maxEvaluatedSolutionsComparator;
      maxEvaluatedSolutionsComparator.Successor = analyzer2;
      analyzer2.Successor = conditionalBranchTerminate;
      conditionalBranchTerminate.FalseBranch = conditionalBranchTerminateSelPressure;
      conditionalBranchTerminate.TrueBranch = null;
      conditionalBranchTerminate.Successor = null;
      conditionalBranchTerminateSelPressure.FalseBranch = conditionalBranchTerminateEvalSolutions;
      conditionalBranchTerminateSelPressure.TrueBranch = null;
      conditionalBranchTerminateSelPressure.Successor = null;
      conditionalBranchTerminateEvalSolutions.FalseBranch = selector;
      conditionalBranchTerminateEvalSolutions.TrueBranch = null;
      conditionalBranchTerminateEvalSolutions.Successor = null;

      #endregion
    }

    public override IOperation Apply() {
      if (MutatorParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}

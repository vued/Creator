﻿#if UNITY_EDITOR

using System.Collections;
using UnityEngine.TestTools;
using System;
using Innoactive.Hub.Training;
using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.Conditions;
using Innoactive.Hub.Training.Configuration;
using Innoactive.Hub.Training.Exceptions;
using UnityEngine;
using NUnit.Framework;

namespace Innoactive.Hub.Unity.Tests.Training
{
    public class StepTests : RuntimeTests
    {
        [UnityTest]
        public IEnumerator StepIsSetup()
        {
            Step step = new Step("Step1");

            // Name should be added to the chapter.
            Assert.AreEqual("Step1", step.Data.Name);

            // State is correct
            Assert.AreEqual(Stage.Inactive, step.LifeCycle.Stage, "Chapter should be inactive");

            // Has transitions and behaviours
            Assert.IsNotNull(step.Data.Behaviors, "Behaviors list should be initialized");
            Assert.IsNotNull(step.Data.Transitions.Data.Transitions, "Transitions list should be initialized");

            yield return null;
        }

        [UnityTest]
        public IEnumerator DeactivateWhileNotActive()
        {
            Step step = new Step("Step1");

            bool didNotFail = false;
            bool isWrongException = false;

            // Expect to fail on calling Deactivate() before activating the Chapter.
            try
            {
                step.LifeCycle.Deactivate();
                didNotFail = true;
            }
            catch (InvalidStateException)
            {
                // This is ok
            }
            catch (Exception)
            {
                isWrongException = true;
            }

            Assert.IsFalse(didNotFail, "No Exception was raised!");
            Assert.IsFalse(isWrongException, "Wrong Exception was raised!");

            yield return null;
        }

        [UnityTest]
        public IEnumerator EmptyStepRaisesEvents()
        {
            Step step = new Step("Step1");
            step.Configure(RuntimeConfigurator.Configuration.GetCurrentMode());

            bool wasActivated = false;
            bool wasDeactivated = false;

            step.LifeCycle.StageChanged += (sender, args) =>
            {
                if (args.Stage == Stage.Active)
                {
                    wasActivated = true;
                }

                if (args.Stage == Stage.Inactive)
                {
                    wasDeactivated = true;
                }
            };

            // Activate should work on simple steps.
            step.LifeCycle.Activate();

            while (step.LifeCycle.Stage != Stage.Active)
            {
                yield return null;
                step.Update();
            }

            step.LifeCycle.Deactivate();

            while (step.LifeCycle.Stage != Stage.Inactive)
            {
                yield return null;
                step.Update();
            }

            // Step should be completed, and have become activated and deactivated.
            Assert.IsTrue(wasActivated, "Step was not activated");
            Assert.IsTrue(wasDeactivated, "Step was not deactivated");
        }

        [UnityTest]
        public IEnumerator ActivateEventEmitted()
        {
            // Setup Step with event listener for checking states.
            Step step = new Step("Step1");
            bool isActivated = false;
            bool isActivationStarted = false;

            step.LifeCycle.StageChanged += (sender, args) =>
            {
                if (args.Stage == Stage.Active)
                {
                    isActivated = true;
                }

                if (args.Stage == Stage.Activating)
                {
                    isActivationStarted = true;
                }
            };

            step.Configure(RuntimeConfigurator.Configuration.GetCurrentMode());

            // Activate should work on simple steps.
            step.LifeCycle.Activate();

            while (step.LifeCycle.Stage != Stage.Active)
            {
                yield return null;
                step.Update();
            }

            Assert.IsTrue(isActivationStarted, "Step was not activated");
            Assert.IsTrue(isActivated, "Step was not set to active");
        }

        [UnityTest]
        public IEnumerator StepWithCondition()
        {
            Step step = new Step("Step1");
            EndlessCondition condition = new EndlessCondition();
            Transition transition = new Transition();
            transition.Data.Conditions.Add(condition);
            step.Data.Transitions.Data.Transitions.Add(transition);
            step.Configure(RuntimeConfigurator.Configuration.GetCurrentMode());

            step.LifeCycle.Activate();

            while (step.LifeCycle.Stage != Stage.Active)
            {
                yield return null;
                step.Update();
            }

            Stage stepInitialStage = step.LifeCycle.Stage;
            Stage conditionInitialStage = condition.LifeCycle.Stage;
            bool conditionIsCompletedInitially = condition.IsCompleted;

            condition.Autocomplete();

            bool conditionIsCompleted = condition.IsCompleted;

            step.LifeCycle.Deactivate();

            while (step.LifeCycle.Stage != Stage.Inactive)
            {
                yield return null;
                step.Update();
            }

            Stage stepStageInEnd = step.LifeCycle.Stage;
            Stage conditionStageInEnd = condition.LifeCycle.Stage;
            bool conditionIsCompletedInEnd = condition.IsCompleted;

            // Check states were correct
            Assert.AreEqual(Stage.Active, stepInitialStage, "Step should be active initially");
            Assert.AreEqual(Stage.Active, conditionInitialStage, "Condition should be active initially");
            Assert.IsFalse(conditionIsCompletedInitially, "Condition should not completed initially");

            Assert.IsTrue(conditionIsCompleted, "Condition should be completed now");

            Assert.AreEqual(Stage.Inactive, stepStageInEnd, "Step should be inactive in the end");
            Assert.AreEqual(Stage.Inactive, conditionStageInEnd, "Condition should not be active in the end");
            Assert.IsTrue(conditionIsCompletedInEnd, "Condition should be completed in the end");

            yield return null;
        }

        [UnityTest]
        public IEnumerator StepWithInitiallyCompletedConditionResetsCondition()
        {
            // Given a step with an already completed condition,
            Step step = new Step("Step1");
            ICondition condition = new EndlessCondition();
            condition.Autocomplete();
            Transition transition = new Transition();
            transition.Data.Conditions.Add(condition);
            step.Data.Transitions.Data.Transitions.Add(transition);
            step.Configure(RuntimeConfigurator.Configuration.GetCurrentMode());

            // When it is activated,
            step.LifeCycle.Activate();

            while (condition.LifeCycle.Stage != Stage.Active)
            {
                yield return null;
                step.Update();
            }

            // Then the condition is reset to not completed.
            Assert.IsFalse(condition.IsCompleted);

            yield return null;
        }

        [UnityTest]
        public IEnumerator ConditionsActivateOnlyAfterBehaviors()
        {
            Step step = new Step("Step1");
            EndlessCondition condition = new EndlessCondition();
            EndlessBehavior behavior = new EndlessBehavior();
            Transition transition = new Transition();
            transition.Data.Conditions.Add(condition);
            step.Data.Transitions.Data.Transitions.Add(transition);
            step.Data.Behaviors.Data.Behaviors.Add(behavior);
            step.Configure(RuntimeConfigurator.Configuration.GetCurrentMode());

            step.LifeCycle.Activate();

            while (behavior.LifeCycle.Stage != Stage.Activating)
            {
                Assert.AreEqual(Stage.Activating, step.LifeCycle.Stage);
                Assert.AreEqual(Stage.Inactive, condition.LifeCycle.Stage);
                yield return null;
                step.Update();
            }

            behavior.LifeCycle.MarkToFastForwardStage(Stage.Activating);

            while (condition.LifeCycle.Stage != Stage.Active)
            {
                Assert.AreEqual(Stage.Activating, step.LifeCycle.Stage);
                yield return null;
                step.Update();
            }
        }

        [UnityTest]
        public IEnumerator ConditionCompletedAfterTimingBehaviorInStep()
        {
            float targetDuration = 0.5f;
            Step step = new Step("Step1");
            ICondition condition = new TimeoutCondition(targetDuration, "Timeout1");
            Transition transition = new Transition();
            IBehavior behavior = new TimeoutBehavior(targetDuration, targetDuration);
            transition.Data.Conditions.Add(condition);
            step.Data.Transitions.Data.Transitions.Add(transition);
            step.Data.Behaviors.Data.Behaviors.Add(behavior);
            step.Configure(RuntimeConfigurator.Configuration.GetCurrentMode());

            step.LifeCycle.Activate();

            // Activation frame
            yield return null;
            step.Update();

            float startTime = Time.time;
            while (behavior.LifeCycle.Stage != Stage.Active)
            {
                yield return null;
                step.Update();
            }

            float behaviorDuration = Time.time - startTime;

            Assert.AreEqual(targetDuration, behaviorDuration,  Time.deltaTime);
            Assert.IsFalse(condition.IsCompleted);

            // Process frames
            yield return null;
            step.Update();
            yield return null;
            step.Update();
            yield return null;
            step.Update();

            startTime = Time.time;
            while (condition.IsCompleted == false)
            {
                yield return null;
                step.Update();
            }

            float conditionDuration = Time.time - startTime;

            Assert.AreEqual(targetDuration, conditionDuration, Time.deltaTime);
            Assert.IsTrue(condition.IsCompleted);
        }

        [UnityTest]
        public IEnumerator ActivateTest()
        {
            // Setup Step with event listener for checking states.
            Step step = new Step("Step1");
            EndlessCondition condition = new EndlessCondition(); // add condition to prevent step from auto-completing on activation

            Transition transition = new Transition();
            transition.Data.Conditions.Add(condition);
            step.Data.Transitions.Data.Transitions.Add(transition);
            step.Configure(RuntimeConfigurator.Configuration.GetCurrentMode());

            // Activate should work on simple steps.
            step.LifeCycle.Activate();

            while (step.LifeCycle.Stage != Stage.Active)
            {
                yield return null;
                step.Update();
            }

            // Chapter should be active now.
            Assert.AreEqual(Stage.Active, step.LifeCycle.Stage, "Step was not activated");
        }

        [UnityTest]
        public IEnumerator FastForwardInactive()
        {
            // Given a step,
            IBehavior behavior = new EndlessBehavior();
            ICondition condition = new EndlessCondition();
            Transition transition = new Transition();
            IStep step = new Step("Step");
            transition.Data.Conditions.Add(condition);
            step.Data.Behaviors.Data.Behaviors.Add(behavior);
            step.Data.Transitions.Data.Transitions.Add(transition);
            step.Configure(RuntimeConfigurator.Configuration.GetCurrentMode());

            // When you fast-forward it,
            step.LifeCycle.MarkToFastForward();

            // Then it doesn't change it's activation state, as well as its contents.
            Assert.AreEqual(Stage.Inactive, step.LifeCycle.Stage);
            Assert.AreEqual(Stage.Inactive, behavior.LifeCycle.Stage);
            Assert.AreEqual(Stage.Inactive, condition.LifeCycle.Stage);

            yield break;
        }

        [UnityTest]
        public IEnumerator FastForwardInactiveAndActivate()
        {
            // Given a step,
            IBehavior behavior = new EndlessBehavior();
            ICondition condition = new EndlessCondition();
            Transition transition = new Transition();
            IStep step = new Step("Step");
            transition.Data.Conditions.Add(condition);
            step.Data.Behaviors.Data.Behaviors.Add(behavior);
            step.Data.Transitions.Data.Transitions.Add(transition);
            step.Configure(RuntimeConfigurator.Configuration.GetCurrentMode());

            // When you fast-forward and activate it,
            step.LifeCycle.MarkToFastForward();
            step.LifeCycle.Activate();

            // Then everything is completed.
            Assert.AreEqual(Stage.Active, step.LifeCycle.Stage);
            Assert.AreEqual(Stage.Active, behavior.LifeCycle.Stage);
            Assert.AreEqual(Stage.Active, condition.LifeCycle.Stage);

            yield break;
        }

        [UnityTest]
        public IEnumerator FastForwardActive()
        {
            // Given a step,
            IBehavior behavior = new EndlessBehavior();
            ICondition condition = new EndlessCondition();
            Transition transition = new Transition();
            IStep step = new Step("Step");
            transition.Data.Conditions.Add(condition);
            step.Data.Behaviors.Data.Behaviors.Add(behavior);
            step.Data.Transitions.Data.Transitions.Add(transition);
            step.Configure(RuntimeConfigurator.Configuration.GetCurrentMode());

            step.LifeCycle.Activate();

            // When you fast-forward it,
            step.LifeCycle.MarkToFastForward();

            // Then everything is completed.
            Assert.AreEqual(Stage.Active, step.LifeCycle.Stage);
            Assert.AreEqual(Stage.Active, behavior.LifeCycle.Stage);
            Assert.AreEqual(Stage.Active, condition.LifeCycle.Stage);

            yield break;
        }
    }
}
#endif

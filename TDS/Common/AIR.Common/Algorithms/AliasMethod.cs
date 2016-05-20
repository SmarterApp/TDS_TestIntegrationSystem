/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Generic;

namespace AIR.Common.Algorithms
{
    /******************************************************************************
     * File: AliasMethod.java
     * Author: Keith Schwarz (htiek@cs.stanford.edu)
     * C# Port: Mike Mariano
    /******************************************************************************/

    /// <summary>
    /// An implementation of the alias method implemented using Vose's algorithm. 
    /// The alias method allows for efficient sampling of random values from a
    /// discrete probability distribution (i.e. rolling a loaded die) in O(1) time
    /// each after O(n) preprocessing time.
    /// </summary>
    /// <remarks>
    /// For a complete writeup on the alias method, including the intuition and
    /// important proofs, please see the article "Darts, Dice, and Coins: Sampling
    /// from a Discrete Distribution" at http://www.keithschwarz.com/darts-dice-coins/
    /// </remarks>
    public class AliasMethod 
    {
        // The random number generator used to sample from the distribution.
        private readonly Random _random;

        // The probability and alias tables.
        private readonly int[] _alias;
        private readonly double[] _probability;

        /// <summary>
        /// Constructs a new AliasMethod to sample from a discrete distribution and
        /// hand back outcomes based on the probability distribution.
        ///
        /// Given as input a list of probabilities corresponding to outcomes 0, 1,
        /// ..., n - 1, this constructor creates the probability and alias tables
        /// needed to efficiently sample from this distribution.
        /// </summary>
        /// <param name="probabilities">The list of probabilities.</param>
        public AliasMethod(List<Double> probabilities) : this(probabilities, new Random())
        {
        }

        /// <summary>
        /// Constructs a new AliasMethod to sample from a discrete distribution and
        /// hand back outcomes based on the probability distribution.
        ///
        /// Given as input a list of probabilities corresponding to outcomes 0, 1,
        /// ..., n - 1, this constructor creates the probability and alias tables
        /// needed to efficiently sample from this distribution.
        /// </summary>
        /// <param name="probabilities">The list of probabilities.</param>
        /// <param name="random">The random number generator.</param>
        public AliasMethod(List<Double> probabilities, Random random) 
        {
            // Begin by doing basic structural checks on the inputs.
            if (probabilities == null || random == null)
            {
                throw new NullReferenceException();
            }

            if (probabilities.Count == 0)
            {
                throw new ArgumentException("Probability vector must be nonempty.");
            }

            // Allocate space for the probability and alias tables.
            _probability = new double[probabilities.Count];
            _alias = new int[probabilities.Count];

            // Store the underlying generator.
            this._random = random;

            // Compute the average probability and cache it for later use.
            double average = 1.0 / probabilities.Count;

            // Make a copy of the probabilities list, since we will be making changes to it.
            probabilities = new List<Double>(probabilities);

            // Create two stacks to act as worklists as we populate the tables.
            Stack<int> small = new Stack<int>();
            Stack<int> large = new Stack<int>();

            // Populate the stacks with the input probabilities.
            for (int i = 0; i < probabilities.Count; ++i) {
                // If the probability is below the average probability, then we add
                // it to the small list; otherwise we add it to the large list.
                if (probabilities[i] >= average)
                {
                    large.Push(i);
                }
                else
                {
                    small.Push(i);
                }
            }

            /* As a note: in the mathematical specification of the algorithm, we
             * will always exhaust the small list before the big list.  However,
             * due to floating point inaccuracies, this is not necessarily true.
             * Consequently, this inner loop (which tries to pair small and large
             * elements) will have to check that both lists aren't empty.
             */
            while (small.Count > 0 && large.Count > 0) {

                // Get the index of the small and the large probabilities.
                int less = small.Pop();
                int more = large.Pop();

                // These probabilities have not yet been scaled up to be such
                // that 1/n is given weight 1.0.  We do this here instead.
                _probability[less] = probabilities[less] * probabilities.Count;
                _alias[less] = more;

                // Decrease the probability of the larger one by the appropriate amount.
                probabilities[more] = ((probabilities[more] + probabilities[less]) - average);

                // If the new probability is less than the average, add it into the
                // small list; otherwise add it to the large list.
                if (probabilities[more] >= 1.0 / probabilities.Count)
                {
                    large.Push(more);
                }
                else
                {
                    small.Push(more);
                }
            }

            /* At this point, everything is in one list, which means that the
             * remaining probabilities should all be 1/n.  Based on this, set them
             * appropriately.  Due to numerical issues, we can't be sure which
             * stack will hold the entries, so we empty both.
             */
            while (small.Count > 0)
            {
                _probability[small.Pop()] = 1.0;
            }
            while (large.Count > 0)
            {
                _probability[large.Pop()] = 1.0;
            }
        }

        /// <summary>
        /// Samples a value from the underlying distribution.
        /// </summary>
        /// <returns>
        /// A random value sampled from the underlying distribution.
        /// </returns>
        public int Next() {

            // Generate a fair die roll to determine which column to inspect.
            int column = _random.Next(_probability.Length);

            // Generate a biased coin toss to determine which option to pick.
            bool coinToss = _random.NextDouble() < _probability[column];

            // Based on the outcome, return either the column or its alias.
            return coinToss ? column : _alias[column];
        }
    }

    /// <summary>
    /// A generics version of alias method algorithm.
    /// </summary>
    public class AliasMethod<T>
    {
        private readonly AliasMethod _aliasMethod;
        private readonly List<T> _list = new List<T>();

        public AliasMethod(List<Tuple<T, Double>> list, Random random)
        {
            List<Double> probabilities = new List<Double>();

            foreach (Tuple<T, Double> tuple in list)
            {
                _list.Add(tuple.Item1);
                probabilities.Add(tuple.Item2);
            }

            _aliasMethod = new AliasMethod(probabilities, random);
        }

        public AliasMethod(List<Tuple<T, Double>> list) : this(list, new Random())
        {
        }

        public T Next()
        {
            return _list[_aliasMethod.Next()];
        }
    }

}

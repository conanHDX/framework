﻿// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.MachineLearning
{
    using Accord.Math;
    using System;

    /// <summary>
    ///   Base class for <see cref="IMultilabelGenerativeClassifier{TInput}">
    ///   generative multi-label classifiers</see>.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    [Serializable]
    public abstract class MultilabelGenerativeClassifierBase<TInput> :
        MultilabelDistanceClassifierBase<TInput>,
        IMultilabelGenerativeClassifier<TInput>
    {

        // Main overridable methods

        /// <summary>
        /// Computes a log-likelihood measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="decision">The class label associated with the input
        ///     vector, as predicted by the classifier.</param>
        /// 
        public abstract double LogLikelihood(TInput input, int classIndex, out bool decision);


        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public virtual double[] Probabilities(TInput input, ref bool[] decision, double[] result)
        {
            LogLikelihoods(input, ref decision, result);
            return Special.Softmax(result, result);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public virtual double[] LogLikelihoods(TInput input, ref bool[] decision, double[] result)
        {
            return Distances(input, ref decision, result);
        }


        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        public virtual double LogLikelihood(TInput input, int classIndex)
        {
            var decision = new bool[NumberOfOutputs];
            var result = new double[NumberOfOutputs];
            LogLikelihoods(input, ref decision, result);
            return result[classIndex];
        }

        /// <summary>
        /// Computes the log-likelihood that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double[] LogLikelihoods(TInput input)
        {
            return LogLikelihoods(input, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public virtual double[] LogLikelihoods(TInput input, double[] result)
        {
            var decision = new bool[NumberOfOutputs];
            return LogLikelihoods(input, ref decision, result);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public virtual double[][] LogLikelihoods(TInput[] input)
        {
            return LogLikelihoods(input, create<double>(input));
        }

        /// <summary>
        /// Computes the log-likelihood that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public virtual double[][] LogLikelihoods(TInput[] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihoods(input[i], result[i]);
            return result;
        }



        // Input, classIndex
        /// <summary>
        /// Computes the probability that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        public double Probability(TInput input, int classIndex)
        {
            return Probabilities(input)[classIndex];
        }

        /// <summary>
        /// Computes the probability that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        public double[] Probability(TInput[] input, int classIndex)
        {
            return Probability(input, classIndex, new double[input.Length]);
        }

        /// <summary>
        /// Computes the probability that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Probability(TInput[] input, int classIndex, double[] result)
        {
            var temp = new double[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                Probabilities(input[i], result: temp);
                result[i] = temp[classIndex];
            }

            return result;
        }


        // Input, classIndex[]
        /// <summary>
        /// Computes the probability that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        public double[] Probability(TInput[] input, int[] classIndex)
        {
            return Probability(input, classIndex, new double[input.Length]);
        }

        /// <summary>
        /// Computes the probability that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Probability(TInput[] input, int[] classIndex, double[] result)
        {
            var temp = new double[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                Probabilities(input[i], result: temp);
                result[i] = temp[classIndex[i]];
            }

            return result;
        }



        // Probabilities        

        /// <summary>
        /// Computes the probabilities that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double[] Probabilities(TInput input)
        {
            return Probabilities(input, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Computes the probabilities that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Probabilities(TInput input, double[] result)
        {
            result = LogLikelihoods(input, result);
            Elementwise.Exp(result, result);
            return result;
        }

        /// <summary>
        /// Computes the probabilities that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double[][] Probabilities(TInput[] input)
        {
            return Probabilities(input, create<double>(input));
        }

        /// <summary>
        /// Computes the probabilities that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Probabilities(TInput[] input, double[][] result)
        {
            result = LogLikelihoods(input, result);
            for (int i = 0; i < input.Length; i++)
                Elementwise.Exp(result[i], result[i]);
            return result;
        }





        // LogLikelihood, input, classIndex

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        public double[] LogLikelihood(TInput[] input, int[] classIndex)
        {
            return LogLikelihood(input, classIndex, new double[input.Length]);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public virtual double[] LogLikelihood(TInput[] input, int[] classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i], classIndex[i]);
            return result;
        }

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        public double[] LogLikelihood(TInput[] input, int classIndex)
        {
            return LogLikelihood(input, classIndex, new double[input.Length]);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] LogLikelihood(TInput[] input, int classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i], classIndex);
            return result;
        }






        // Same as Distance methods in IMultilabelDistanceClassifier, 
        // but replaced with LogLikelihood

        #region LogLikelihoods


        // Input, decision

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] LogLikelihoods(TInput input, out double decision)
        {
            return LogLikelihoods(input, out decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] LogLikelihoods(TInput input, out int decision)
        {
            return LogLikelihoods(input, out decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] LogLikelihoods(TInput input, ref bool[] decision)
        {
            return LogLikelihoods(input, ref decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        public double[] LogLikelihoods(TInput input, ref int[] decision)
        {
            return LogLikelihoods(input, ref decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        public double[] LogLikelihoods(TInput input, ref double[] decision)
        {
            return LogLikelihoods(input, ref decision, new double[NumberOfOutputs]);
        }


        // Input, decision, result

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] LogLikelihoods(TInput input, out double decision, double[] result)
        {
            LogLikelihoods(input, result);
            decision = result.ArgMax();
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] LogLikelihoods(TInput input, out int decision, double[] result)
        {
            LogLikelihoods(input, result);
            decision = result.ArgMax();
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] LogLikelihoods(TInput input, ref int[] decision, double[] result)
        {
            decision = create(input, decision);
            var mask = new bool[NumberOfOutputs];
            LogLikelihoods(input, ref mask, result);
            Vector.KHot<int>(mask, decision);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] LogLikelihoods(TInput input, ref double[] decision, double[] result)
        {
            decision = create(input, decision);
            var mask = new bool[NumberOfOutputs];
            LogLikelihoods(input, ref mask, result);
            Vector.KHot<double>(mask, decision);
            return result;
        }



        // Input[], decision[]

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] LogLikelihoods(TInput[] input, ref double[] decision)
        {
            return LogLikelihoods(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] LogLikelihoods(TInput[] input, ref int[] decision)
        {
            return LogLikelihoods(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] LogLikelihoods(TInput[] input, ref int[][] decision)
        {
            return LogLikelihoods(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] LogLikelihoods(TInput[] input, ref bool[][] decision)
        {
            return LogLikelihoods(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] LogLikelihoods(TInput[] input, ref double[][] decision)
        {
            return LogLikelihoods(input, ref decision, create<double>(input));
        }




        // Input[], decision[], result[]        

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] LogLikelihoods(TInput[] input, ref double[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                LogLikelihoods(input[i], out decision[i], result[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] LogLikelihoods(TInput[] input, ref int[] decision, double[][] result)
        {
            LogLikelihoods(input, result);
            result.ArgMax(dimension: 1, result: decision);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] LogLikelihoods(TInput[] input, ref bool[][] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                LogLikelihoods(input[i], ref decision[i], result[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] LogLikelihoods(TInput[] input, ref int[][] decision, double[][] result)
        {
            decision = create(input, decision);
            var mask = new bool[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                LogLikelihoods(input[i], ref mask, result[i]);
                decision[i] = Vector.KHot<int>(mask, decision[i]);
            }
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] LogLikelihoods(TInput[] input, ref double[][] decision, double[][] result)
        {
            decision = create(input, decision);
            var mask = new bool[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                LogLikelihoods(input[i], ref mask, result[i]);
                decision[i] = Vector.KHot<double>(mask, decision[i]);
            }
            return result;
        }

        #endregion




        // Same as Distance methods in IMultilabelDistanceClassifier, 
        // but replaced with Probabilities

        #region Probabilities


        // Input, decision

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] Probabilities(TInput input, out double decision)
        {
            return Probabilities(input, out decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] Probabilities(TInput input, out int decision)
        {
            return Probabilities(input, out decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        public double[] Probabilities(TInput input, ref bool[] decision)
        {
            return Probabilities(input, ref decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        public double[] Probabilities(TInput input, ref int[] decision)
        {
            return Probabilities(input, ref decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        public double[] Probabilities(TInput input, ref double[] decision)
        {
            return Probabilities(input, ref decision, new double[NumberOfOutputs]);
        }


        // Input, decision, result

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Probabilities(TInput input, out double decision, double[] result)
        {
            Probabilities(input, result);
            decision = result.ArgMax();
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Probabilities(TInput input, out int decision, double[] result)
        {
            Probabilities(input, result);
            decision = result.ArgMax();
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Probabilities(TInput input, ref int[] decision, double[] result)
        {
            decision = create(input, decision);
            var mask = new bool[NumberOfOutputs];
            Probabilities(input, ref mask, result);
            Vector.KHot<int>(mask, decision);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Probabilities(TInput input, ref double[] decision, double[] result)
        {
            decision = create(input, decision);
            var mask = new bool[NumberOfOutputs];
            Probabilities(input, ref mask, result);
            Vector.KHot<double>(mask, decision);
            return result;
        }



        // Input[], decision[]

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] Probabilities(TInput[] input, ref double[] decision)
        {
            return Probabilities(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] Probabilities(TInput[] input, ref int[] decision)
        {
            return Probabilities(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] Probabilities(TInput[] input, ref int[][] decision)
        {
            return Probabilities(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] Probabilities(TInput[] input, ref bool[][] decision)
        {
            return Probabilities(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] Probabilities(TInput[] input, ref double[][] decision)
        {
            return Probabilities(input, ref decision, create<double>(input));
        }




        // Input, decision, result        

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Probabilities(TInput[] input, ref double[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                Probabilities(input[i], out decision[i], result[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Probabilities(TInput[] input, ref int[] decision, double[][] result)
        {
            Probabilities(input, result);
            result.ArgMax(dimension: 1, result: decision);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Probabilities(TInput[] input, ref bool[][] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                Probabilities(input[i], ref decision[i], result[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Probabilities(TInput[] input, ref int[][] decision, double[][] result)
        {
            decision = create(input, decision);
            var mask = new bool[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                Probabilities(input[i], ref mask, result[i]);
                decision[i] = Vector.KHot<int>(mask, decision[i]);
            }
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Probabilities(TInput[] input, ref double[][] decision, double[][] result)
        {
            decision = create(input, decision);
            var mask = new bool[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                Probabilities(input[i], ref mask, result[i]);
                decision[i] = Vector.KHot<double>(mask, decision[i]);
            }
            return result;
        }

        #endregion


        // Transform

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <param name="result">A location to store the output, avoiding unnecessary memory allocations.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public override double[] Transform(TInput input, double[] result)
        {
            return Probabilities(input, result);
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <param name="result">A location to store the output, avoiding unnecessary memory allocations.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public override double[][] Transform(TInput[] input, double[][] result)
        {
            return Probabilities(input, result);
        }


    }
}
/* Bayes class
 * 
 * Handles bayesian decision making
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BayesDemo
{
	/***** Global data types used in this class *****/

	// Made a struct because it's faster/smaller
	public struct Observation
	{
		/*public Outlook outlook;		// 3 legal choices
		public int closestToDragon;			// degrees Farenheit
		public int closestToTower;			// Relative closestToTowerity as %
		public bool windy;			// Windy or not
		public bool play;			// Play or not*/

		public float closestToTower;
		public int numByTower;
		public float closestToDragon;
		public int numByDragon;
		public float dragonDistFromTower;
		public bool chooseTargetClosestToTowerGood;
	}

	/*********************************************************************/

	public class Bayes
	{
		// Calculate the constant once
		double sqrt2PI = Math.Sqrt (2.0 * Math.PI);

		// List of observations.  Initialized from the data file.
		// Added to with new observations during program run
		List<Observation> obsTab = new List<Observation> ();

		// All the little tables to store the counts, proportions,
		// sums, sums of squares, means, and standard deviations
		// for the 4 conditions and the action.  Used doubles for the proportions,
		// means and standard deviations to mitigate roundoff errors for products
		// of small probabilities.

		// tower condition (continuous) needs sum, sum of squares for mean, StdDev
		float[] towerSum = new float[2];				// Stats for 2 outcomes (T or F)
		float[] towerSumSq = new float[2];
		double[] towerMean = new double[2];
		double[] towerStdDev = new double[2];

		// number by tower condition (continuous)
		int[] numTowerSum = new int[2];
		int[] numTowerSumSq = new int[2];
		double[] numTowerMean = new double[2];
		double[] numTowerStdDev = new double[2];

		// dragon condition (continuous)
		float[] dragonSum = new float[2];
		float[] dragonSumSq = new float[2];
		double[] dragonMean = new double[2];
		double[] dragonStdDev = new double[2];

		// number by dragon condition (continuous)
		int[] numDragonSum = new int[2];
		int[] numDragonSumSq = new int[2];
		double[] numDragonMean = new double[2];
		double[] numDragonStdDev = new double[2];

		// tertiary condition continuous
		float[] dragonDistToTowerSum = new float[2];
		float[] dragonDistToTowerSumSq = new float[2];
		double[] dragonDistToTowerMean = new double[2];
		double[] dragonDistToTowerStdDev = new double[2];

		int[] choseTowerCt = new int[2];
		double[] choseTowerPrp = new double[2];

		/************************************************************************/

		// Constructor just makes sure all the stat tables are initialized
		public Bayes ()
		{
			InitStats ();
		}

		// Need to (re)initialize accumulators now and then
		void InitStats ()
		{
			/*for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 2; j++) {
					outlookCt [i, j] = 0;
				}
			}

			for (int i = 0; i < dragonSum.Length; i++) {
				dragonSum [i] = 0;
				dragonSumSq [i] = 0;
			}

			for (int i = 0; i < towerSum.Length; i++) {
				towerSum [i] = 0;
				towerSumSq [i] = 0;
			}

			for (int i = 0; i < 2; i++) {
				for (int j = 0; j < 2; j++) {
					windyCt [i, j] = 0;
				}
			}

			for (int i = 0; i < 2; i++) {
				choseTowerCt [i] = 0;
			}*/

			for (int i = 0; i < 2; i++) {
				towerSum [i] = 0;
				towerSumSq [i] = 0;
			}

			for (int i = 0; i < 2; i++) {
				numTowerSum [i] = 0;
				numTowerSumSq [i] = 0;
			}

			for (int i = 0; i < 2; i++) {
				dragonSum [i] = 0;
				dragonSumSq [i] = 0;
			}

			for (int i = 0; i < 2; i++) {
				numDragonSum [i] = 0;
				numDragonSumSq [i] = 0;
			}

			for (int i = 2; i < 2; i++) {
				dragonDistToTowerSum [i] = 0;
				dragonDistToTowerSumSq [i] = 0;
			}

			for (int i = 0; i < 2; i++) {
				choseTowerCt [i] = 0;
			}
		}

		// Read the observations from text file fName.txt and add them to the list
		public void ReadObsTab (string fName)
		{
			try {
				using (StreamReader rdr = new StreamReader (fName)) {
					string lineBuf = null;
					while ((lineBuf = rdr.ReadLine ()) != null)
					{
						string[] lineAra = lineBuf.Split (' ');

						// Map strings to correct data types for conditions & action
						// and Add the observation to List obsTab
						AddObs (float.Parse (lineAra [0]), int.Parse (lineAra [1]), float.Parse (lineAra [2]), int.Parse (lineAra [3]), float.Parse(lineAra[4]), (lineAra [5] == "True" ? true : false));
					}
				}
			}
			catch {
				Console.WriteLine ("Problem reading and/or parsing observations in " + fName);
				Environment.Exit (-1);
			}
		}

		// Add an observation to the list.
		// Used when reading the file and when adding new observations on the fly
		public void AddObs (float closestToDragon, int numByDragon, float closestToTower, int numByTower, float dragonDistToTower, bool chooseTargetClosestToTowerGood)
		{
			// Build an Observation struct
			Observation obs;
			obs.closestToDragon = closestToDragon;
			obs.numByDragon = numByDragon;
			obs.closestToTower = closestToTower;
			obs.numByTower = numByTower;
			obs.dragonDistFromTower = dragonDistToTower;
			obs.chooseTargetClosestToTowerGood = chooseTargetClosestToTowerGood;

			// Add it to the List
			obsTab.Add (obs);
		}

		// Dump obsTab to text file fName so it can be read next time
		public void Tab2File (string fName)
		{
			try {
				using (StreamWriter wtr = new StreamWriter (fName)) {
					foreach (Observation obs in obsTab)
					{
						wtr.Write ("{0}", obs.closestToDragon);
						wtr.Write (" {0}", obs.numByDragon);
						wtr.Write (" {0}", obs.closestToTower);
						wtr.Write (" {0}", obs.numByTower);
						wtr.Write (" {0}", obs.dragonDistFromTower);
						wtr.WriteLine (" {0}", obs.chooseTargetClosestToTowerGood);
					}
				}
			}
			catch {
				Console.WriteLine ("Problem writing out the Observations to " + fName);
				Console.WriteLine ("File not changed.");
			}
		}

		// Dump obsTab to the Console window for debugging purposes
		public void Tab2Screen ()
		{
			Console.WriteLine ("Drgn D_Num Towr T_Num | T_Targ?");
			Console.WriteLine ("------------------------------");
			foreach (Observation obs in obsTab)
			{
				Console.Write ("{0,-8}", obs.closestToDragon);
				Console.Write (" " + obs.numByDragon);
				Console.Write ("  " + obs.closestToDragon);
				Console.Write ("  {0,-5}", obs.numByTower);
				Console.Write ("  {0,-5}", obs.dragonDistFromTower);
				Console.WriteLine (" | {0,-5}", obs.chooseTargetClosestToTowerGood);
			}
		}

		/***************************************************************************/

		// Build all the statistics needed for Bayes from the observations
		// in obsTab.  Presumably, this would be called during initialization
		// and not after every new observation has been added durng game play,
		// as it does a lot of crunching on doubles.  With a small obsTab
		// this may not be much of an issue, but as it grows O(n) with size of
		// obsTab, it could pork out with a lot of new observations added during
		// game play.
		//
		// Could implement an UpdateStats() method that incrementally bumped the
		// accumulators when a new observation is added before recalculating the
		// stats, which would eliminate the observation loop,
		// but there still would be some crunching going on.
		public void BuildStats ()
		{
			InitStats ();		// Reset all the accumulators

			// Accumulate all the counts and sums
			foreach (Observation obs in obsTab) {
				// Do this once
				int targetTower = obs.chooseTargetClosestToTowerGood ? 0 : 1;

				dragonSum [targetTower] += obs.closestToDragon;
				dragonSumSq [targetTower] += obs.closestToDragon * obs.closestToDragon;

				numDragonSum [targetTower] += obs.numByDragon;
				numDragonSumSq [targetTower] += obs.numByDragon * obs.numByDragon;

				towerSum [targetTower] += obs.closestToTower;
				towerSumSq [targetTower] += obs.closestToTower * obs.closestToTower;

				numTowerSum [targetTower] += obs.numByTower;
				numTowerSumSq [targetTower] += obs.numByTower * obs.numByTower;

				dragonDistToTowerSum [targetTower] += obs.dragonDistFromTower;
				dragonDistToTowerSumSq [targetTower] += obs.dragonDistFromTower * obs.dragonDistFromTower;

				choseTowerCt [targetTower]++;
			}

			// Calculate all the statistics
			//CalcProps (outlookCt, choseTowerCt, outlookPrp);

			dragonMean [0] = Mean (dragonSum [0], choseTowerCt [0]);
			dragonMean [1] = Mean (dragonSum [1], choseTowerCt [1]);
			dragonStdDev [0] = StdDev (dragonSumSq [0], dragonSum [0], choseTowerCt [0]);
			dragonStdDev [1] = StdDev (dragonSumSq [1], dragonSum [1], choseTowerCt [1]);

			towerMean [0] = Mean (towerSum [0], choseTowerCt [0]);
			towerMean [1] = Mean (towerSum [1], choseTowerCt [1]);
			towerStdDev [0] = StdDev (towerSumSq [0], towerSum [0], choseTowerCt [0]);
			towerStdDev [1] = StdDev (towerSumSq [1], towerSum [1], choseTowerCt [1]);

			numDragonMean [0] = Mean (numDragonSum [0], choseTowerCt [0]);
			numDragonMean [1] = Mean (numDragonSum [1], choseTowerCt [1]);
			numDragonStdDev [0] = StdDev (numDragonSumSq [0], numDragonSum [0], choseTowerCt [0]);
			numDragonStdDev [1] = StdDev (numDragonSumSq [1], numDragonSum [1], choseTowerCt [1]);

			numTowerMean [0] = Mean (numTowerSum [0], choseTowerCt [0]);
			numTowerMean [1] = Mean (numTowerSum [1], choseTowerCt [1]);
			numTowerStdDev [0] = StdDev (numTowerSumSq [0], numTowerSum [0], choseTowerCt [0]);
			numTowerStdDev [1] = StdDev (numTowerSumSq [1], numTowerSum [1], choseTowerCt [1]);

			dragonDistToTowerMean [0] = Mean (dragonDistToTowerSum [0], choseTowerCt [0]);
			dragonDistToTowerMean [1] = Mean (dragonDistToTowerSum [1], choseTowerCt [1]);
			dragonDistToTowerStdDev [0] = Mean (dragonDistToTowerSumSq [0], choseTowerCt [0]);
			dragonDistToTowerStdDev [1] = Mean (dragonDistToTowerSumSq [1], choseTowerCt [1]);

			//CalcProps (windyCt, choseTowerCt, windyPrp);

			choseTowerPrp [0] = (double)choseTowerCt [0] / obsTab.Count;
			choseTowerPrp [1] = (double)choseTowerCt [1] / obsTab.Count;
		}

		// Standard statistical functions.  These should be useful without modification.

		// Calculates the proportions for a discrete table of counts
		// Handles the 0-frequency problem by assigning an artificially
		// low value that is still greater than 0.
		void CalcProps (int[,] counts, int[] n, double[,] props)
		{
			for (int i = 0; i < counts.GetLength (0); i++)
				for (int j = 0; j < counts.GetLength (1); j++)
					// Detects and corrects a 0 count by assigning a proportion
					// that is 1/10 the size of a proportion for a count of 1
					if (counts [i, j] == 0)
						props [i, j] = 0.1d / choseTowerCt [j];	// Can't have 0
					else
						props [i, j] = (double)counts [i, j] / n [j];
		}

		double Mean (int sum, int n)
		{
			return (double)sum / n;
		}

		double Mean (float sum, int n)
		{
			return (double)sum / n;
		}

		double StdDev (int sumSq, int sum, int n)
		{
			return Math.Sqrt ((sumSq - (sum * sum) / (double)n) / (n - 1));
		}

		double StdDev (float sumSq, float sum, int n)
		{
			return Math.Sqrt ((sumSq - (sum * sum) / (double)n) / (n - 1));
		}

		// Calculates probability of x in a normal distribution of
		// mean and stdDev.  This corrects a mistake in the pseudo-code,
		// which used a power function instead of an exponential.
		double GauProb (double mean, double stdDev, int x)
		{
			double xMinusMean = x - mean;
			return (1.0d / (stdDev * sqrt2PI)) *
				Math.Exp (-1.0d * xMinusMean * xMinusMean / (2.0d * stdDev * stdDev));
		}

		double GauProb (double mean, double stdDev, float x)
		{
			double xMinusMean = x - mean;
			return (1.0d / (stdDev * sqrt2PI)) *
				Math.Exp (-1.0d * xMinusMean * xMinusMean / (2.0d * stdDev * stdDev));
		}

		/*********************************************************/

		// Bayes likelihood for four condition values and one action value
		// For each possible action value, call this with a specific set of four
		// condition values, and pick the action that returns the highest
		// likelihood as the most likely action to take, given the conditions.
		double CalcBayes (float closestToTower, int numByTower, float closestToDragon, int numByDragon, float dragonDistToTower, bool choseTower)
		{
			int playOff = choseTower ? 0 : 1;
			double like =
				GauProb (dragonMean [playOff], dragonStdDev [playOff], closestToDragon) *
				GauProb (towerMean [playOff], towerStdDev [playOff], closestToTower) *
				GauProb (numDragonMean [playOff], numDragonStdDev [playOff], numByDragon) *
				GauProb (numTowerMean [playOff], numTowerStdDev [playOff], numByTower) *
				GauProb (dragonDistToTowerMean[playOff], dragonDistToTowerStdDev[playOff], dragonDistToTower) *	
				choseTowerPrp [playOff];
			return like;
		}

		// Decide whether to play or not.
		// Returns true if decision is to play, false o/w
		// Can turn on/off diagnostic output to Console by playing with "*/"
		//public bool Decide (Outlook ol, int closestToDragon, int hum, bool windy)
		public bool Decide (float closestToDragon, int numByDragon, float closestToTower, int numByTower, float dragonDistToTower)
		{
			//double playYes = CalcBayes (ol, closestToDragon, hum, windy, true);
			double playYes = CalcBayes (closestToTower, numByTower, closestToDragon, numByDragon, dragonDistToTower, true);
			double playNo = CalcBayes (closestToTower, numByTower, closestToDragon, numByDragon, dragonDistToTower, false);
			//double playNo = CalcBayes (ol, closestToDragon, hum, windy, false);

			/* To turn off output, remove this end comment -> */
			double yesNno = playYes + playNo;
			Debug.Log ("playYes: {0}" + playYes);	// Use scientifice notation
			Console.WriteLine ("playNo:  {0}", playNo);		// for very small numbers
			Console.WriteLine ("playYes Normalized: {0,6:F4}", playYes / yesNno);
			Console.WriteLine ("playNo  Normalized: {0,6:F4}", playNo / yesNno);
			/* */

			return playYes > playNo;
		}

		/**************************************************************************/

		// Dump all the statistics to the Console for debugging purposes
		public void DumpStats ()
		{
			Console.WriteLine ("Overall Play Outcomes:");
			Console.WriteLine ("#True  {0}  Proportion {1:F3}",
				choseTowerCt [0], choseTowerPrp [0]);
			Console.WriteLine ("#False {0}  Proportion {1:F3}",
				choseTowerCt [1], choseTowerPrp [1]);
			Console.WriteLine ();

			Console.WriteLine ("Outlook: ");
			Console.WriteLine ("Value    #T  p of T  #F  p of F");
			for (int i = 0; i < 3; i++) {
				/*Console.Write ("{0,-8}", (Outlook)i);
				for (int j = 0; j < 2; j++) {
					Console.Write ("  {0:D}", outlookCt [i, j]);
					Console.Write ("  {0:F4} ", outlookPrp [i, j]);
				}*/
				Console.WriteLine ();
			}


			Console.WriteLine ();
			Console.WriteLine ("closestToDragon:");
			DumpContAttr (dragonMean [0], dragonMean [1], dragonStdDev [0], dragonStdDev [1]);

			Console.WriteLine ();
			Console.WriteLine ("closestToTower:");
			DumpContAttr (towerMean [0], towerMean [1], towerStdDev [0], towerStdDev [1]);

			Console.WriteLine ();
			Console.WriteLine ("Windy:");
			Console.WriteLine ("Value    #T  p of T  #F  p of F");
			for (int i = 0; i < 2; i++)
			{
				if (i == 0)
					Console.Write ("{0,-8}", true);		// Kludgy way to show boolean
				else
					Console.Write ("{0,-8}", false);
				for (int j = 0; j < 2; j++)
				{
					//Console.Write ("  {0:D}", windyCt [i, j]);
					//Console.Write ("  {0:F4} ", windyPrp [i, j]);
				}
				Console.WriteLine ();
			}
		}

		// Dump a continuous attribute's statistics
		void DumpContAttr (double mT, double mF, double sdT, double sdF)
		{
			Console.WriteLine (" MeanT   MeanF   StDvT   StDvF");
			Console.Write ("{0,6:F2}  {1,6:F2} ", mT, mF);
			Console.WriteLine (" {0,6:F2}  {1,6:F2} ", sdT, sdF);
		}
	}
}


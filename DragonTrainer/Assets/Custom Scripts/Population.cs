using System;
using System.IO;
using UnityEngine;

namespace ThreshEvolve
{
	/* Population class implements a single population. Used by ThreshPop for both
	 * oldP and newP. 
	 */
	public class Population
	{
		int popSize;			// Population size
		int nBits;           	// Number of bits per chromosome
		int nChromVals;      	// Number of different chromosome values (2 to the nBits)
		Individual [] dudes;	// Array of Individuals
		int nDudes = 0;			// Current number of Individuals
		float totFit = 0;      	// Total fitness for all individuals in population
		double fitMean = 0.0;	// Mean fitness value
		double fitStdDev = 0.0;	// Standard deviation of the fitness distribution
		char[] delim = {' '};	// Used in ReadPop to split input lines
		double crossoverProb;	// chance of crossover in used in BreedDude()
		double mutationProb;	// chance of mutation in Individual.Mutate()

		// Constructor sets up an empty population with popN individuals,
		//   chromosome length of cLeng (sets iBits), and crossover & mutation
		//   probabilities.
		public Population (int popN, int cLeng, double pXOver, double pMut)
		{
			popSize = popN;
			dudes = new Individual[popSize];
			nDudes = 0;
			nBits = cLeng;
			nChromVals = 1 << nBits;
			totFit = 0;
			crossoverProb = pXOver;
			mutationProb = pMut;
		}

		// Returns true if population is full
		public bool Full
		{
			get { return nDudes == popSize; }
		}

		// Fills population with new random chromosomes for generation 0
		public void InitPop()
		{
			for (int i = 0; i < popSize; i++)
			{
				dudes[i] = new Individual ((uint)Math.Floor(UnityEngine.Random.value*nChromVals), nBits, mutationProb);
			}
			nDudes = popSize;
			totFit = popSize;      // Default fitness for each Individual == 1
		}

		// Fills population by reading individuals from a file already opened to inStream
		// Assumes file is correctly formatted with correct number of lines
		public void ReadPop(StreamReader inStream)
		{
			for (int i = 0; i < popSize; i++)
			{
				string line = inStream.ReadLine();		// Read a line
				string [] tokens = line.Split (delim);	// Split into "words"
				uint chr = UInt32.Parse(tokens[0]);		// Convert words to numbers
				int fit = int.Parse(tokens[1]);
				// Put Individual in population
				dudes [i] = new Individual (chr, nBits, fit, mutationProb);
				totFit += fit;							// Accumulate total fitness for selection
			}
			nDudes = popSize;							// Show the population full
		}

		// Write the population out to a data file that can be read by ReadPop
		public void WritePop(StreamWriter outStream)
		{
			for (int i = 0; i < nDudes; i++)
			{
				outStream.WriteLine (dudes[i]);
			}
		}

		// Calculate fitness mean and standard deviation.  Called from DisplayPop()
		private void PopStats()
		{
			float sumFit = 0;
			float sumSqFit = 0;
			for (int i = 0; i < popSize; i++)
			{
				sumFit += dudes[i].Fitness;
				sumSqFit += dudes[i].Fitness * dudes[i].Fitness;
			}
			fitMean = (double)sumFit / popSize;
			fitStdDev =
				Math.Sqrt((sumSqFit - (sumFit*sumFit)/(double)popSize) / (popSize-1));
		}
			
		// Display the Population on the Console with mean, standard deviation, and best
		// individual
		public void DisplayPop()
		{
			for (int i = 0; i < nDudes; i++)
			{
				Console.WriteLine (dudes [i]);
			}
			PopStats();
			Console.WriteLine ("Average fitness = " + fitMean);
			Console.WriteLine ("Fitness standard deviation = " + fitStdDev);
			Individual best = BestDude();
			Console.WriteLine ("Best dude: " + best.Chrom + " with fitness " +
				best.Fitness);
		}

		// Find the best (highest fitness) Individual in the population
		// Used to implement elitism => best of old Pop survives in new
		public Individual BestDude ()
		{
			// Initialize to the first Individual in the array
			int whereBest = 0;			// Initialze to the first one
			float bestFit = dudes[0].Fitness;

			// Walk through the rest to get the overall best one
			for (int i = 1; i < nDudes; i++)
				if (dudes [i].Fitness > bestFit)
				{
					whereBest = i;
					bestFit = dudes [i].Fitness;
				}
			return dudes[whereBest];
		}

		// Breed a new Individual using crossover and mutation
		public Individual BreedDude()
		{
			// Two selection methods to choose from.  Typically use one or the other
			// but not both.  Used both here just to test them.
			Individual p1 = SelectRoul();				// Get 2 parents
			Individual p2 = SelectTourn();
			uint c1 = p1.Chrom;							// Extract their chromosomes
			uint c2 = p2.Chrom;

			if (UnityEngine.Random.value < crossoverProb) // Probably do crossover
			{
				uint kidChrom = CrossOver(c1, c2);		// Make new chromosome
				Individual newDude = new Individual (kidChrom, nBits, mutationProb);
				// Make Individual
				newDude.Mutate();						// Maybe mutate a bit
				return newDude;							// Send it back
			}
			else
				// No crossover => Pick one of the parents to return unchanged
				return (UnityEngine.Random.value < 0.5 ? p1 : p2);
		}

		// Roulette-wheel selection selects in linear proportion to fitness
		// Uses totFit, which was accumulated when population was filled
		public Individual SelectRoul()
		{
			// Roll a random integer from 0 to totFit - 1
			int roll = (int)Math.Floor(UnityEngine.Random.value*totFit);

			// Walk through the population accumulating fitness
			float accum = dudes[0].Fitness;	// Initialize to the first one
			int iSel = 0;
			// until the accumulator passes the rolled value
			while (accum <= roll && iSel < nDudes-1)
			{
				iSel++;
				accum += dudes[iSel].Fitness;
			}
			// Return the Individual where we stopped
			return dudes[iSel];
		}

		// Tournament Selection selects the winner (highest fitness) of a tournament
		// of randomly chosen contestants.  The size of the tournament controls the
		// selection pressure, with larger tournaments yielding more fit Individuals
		// on the average.
		public Individual SelectTourn()
		{
			const int TOURN_SIZE = 2; 
			Individual next;
			Individual best = dudes[(int)Math.Floor(UnityEngine.Random.value*dudes.Length)];

			for (int i = 1; i < TOURN_SIZE; i++)
			{
				next = dudes[(int)Math.Floor(UnityEngine.Random.value*dudes.Length)];
				if (next.Fitness > best.Fitness)
					best = next;
			}
			return best;
		}
			
		// Single-point crossover of two parents, returns new kid
		// Uses bit shift tricks to get each parent's contribution on opposite
		// sides of random crossover point.  Called by BreedDude().
		private uint CrossOver(uint p1, uint p2)
		{
			int xOverPt = (int)Math.Floor(UnityEngine.Random.value*nBits);	// Pick random crossover point
			p1 = (p1 >> xOverPt) << xOverPt;			// Get p1's bits to the left
			p2 = (p2 << (32 - xOverPt)) >> (32 - xOverPt); // p2's to the right
			uint newKid = p1 | p2;						// Or them together
			return newKid;
		}

		// Add a new Individual to the population in the next open spot
		public int AddNewInd (Individual newDude)
		{
			int wherePut = -1;			// -1 in case something breaks
			if (Full)
				Console.WriteLine ("Panic!  Tried to add too many dudes");
			else
			{
				wherePut = nDudes;
				dudes[wherePut] = newDude;
				nDudes++;				// Increment for next time
			}
			return wherePut;			// Return offset in array where it landed
		}

		// Get Individual at offset where in the array
		public Individual GetDude (int where)
		{
			return dudes [where];
		}

		// Set fitness of Individual at offset where to fitVal
		public void SetFitness (int where, float fitVal)
		{
			dudes[where].Fitness = fitVal;
		}
	}
}


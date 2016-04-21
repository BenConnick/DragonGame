using System;
using System.IO;

namespace ThreshEvolve
{
	/* ThreshPop - Highest level class for the threshold GA package
	 * Implements a single generation of evolution with an old population from
	 * a previous generation (or the initial generation 0), and a new population
	 * created for this generation. Provides all the methods called directly to
	 * implement a threshold population from a calling class.
	 * Uses the Population class to implement its two internal populations
	 * (oldP and newP). Uses the Individual class as well for handling single
	 * Individuals. 
	 */
	public class ThreshPop
	{
		int popSize;		// Number of Individuals in population
		int chromSize;		// Length of chromosome for each individual
		Population oldP;	// Old population read from file or generated randomly
		Population newP;	// New population filled  as Individuals get fitness
		string popPath;		// String for data file path name (in Bin/Debug folder)
		int nextCOut = 0;	// Counter for number of Individuals checked out of oldP
		int nextCIn = 0;	// Counter for number of Individuals checked into of newP
		bool isGeneration0 = false;	// Assume there's a data file from a previous run
		double xOverProb = 0.9;
		double mutProb = 0.1;

		// Constructor sets up old and new populations
		public ThreshPop (int cSize, int size, string path, double pXOver, double pMut)
		{
			popSize = size;
			chromSize = cSize;
			popPath = path;
			xOverProb = pXOver;
			mutProb = pMut;
			// Old population for check out
			oldP = new Population (popSize, chromSize, xOverProb, mutProb);
			FillPop();
			// New population for check in
			newP = new Population (popSize, chromSize, xOverProb, mutProb);
		}

		// Fill oldP either from data file or from scratch (new, random)
		void FillPop ()
		{
			StreamReader inStream = null;	// Open file if it's there
			try
			{
				inStream = new StreamReader(popPath);
				oldP.ReadPop(inStream);		// File opened so read it
				inStream.Close();
			}
			catch
			{
				oldP.InitPop();			// File didn't open so fill with newbies
				isGeneration0 = true;	// Set flag to show it's generation 0
			}
		}

		public void WritePop()
		{
			StreamWriter outStream = new StreamWriter(popPath, false);
			newP.WritePop(outStream);
			outStream.Close();
		}

		// Display either oldP (0) or newP (1) on Console window
		public void DisplayPop(int which)
		{
			if (which == 0)
				oldP.DisplayPop();
			else
				newP.DisplayPop();
		}

		// Check out an individual to use for a threshold in an NPC
		public uint CheckOut ()
		{
			if (isGeneration0)	// Brand new => don't breed
			{
				Individual dude = oldP.GetDude(nextCOut);
				nextCOut++;
				return dude.Chrom;
			}
			else
			{	// Came from file so breed new one
				Individual newDude;
				if (nextCOut == 0)	// First one needs to be Best (elitism)
					newDude = oldP.BestDude();
				else
					newDude = oldP.BreedDude();	// Rest are bred
				nextCOut++;						// Count it
				return newDude.Chrom;			// Return its chromosome
			}
		}

		// Returns true if we've checked out a population's worth
		public bool AllCheckedOut()
		{
			return nextCOut == popSize;
		}

		// Check in an individual that has now acquired a fitness value
		public void CheckIn (uint chr, int fit)
		{
			// Make Individual
			Individual NewDude = new Individual(chr, chromSize, fit, mutProb);
			newP.AddNewInd(NewDude);						// Add to newP
			nextCIn++;										// Count it
			//Console.WriteLine("In CheckIn chr fit: " + chr + " " + fit);
		}

		// Returns true if newP is full of checked in Individuals
		public bool AllCheckedIn()
		{
			return nextCIn == popSize;
		}

	}
}


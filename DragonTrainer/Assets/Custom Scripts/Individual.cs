using System;
using UnityEngine;

namespace ThreshEvolve
{
	/* Individual class implements a single Individual from a population.
	 * In addition to being the base type for the Population array, it is
	 * used to pass Individuals around in Population and ThreshPop, but it
	 * is not used by the Main method.  Contains the mutation operator, since
 	 * mutation only involves a single individual.
	 */
	public class Individual
	{
		double mutProb;	// Mutation probability used in Mutate()
		float fitness;
		uint chrom;		// up to 32-bit chromosome with an unsigned integer
		int nBits;		// Number of bits actually used (starting w/ least sig)

		// Called by InitPop() with no fitness value
		public Individual (uint newChrom, int nB, double pMut)
		{
			chrom = newChrom;
			nBits = nB;
			fitness = 1;		// Default fitness must be non-zero
			mutProb = pMut;
		}

		// Overload called by ReadPop() with fitness from last generation
		public Individual (uint newChrom, int nB, float fit, double pMut)
		{
			chrom = newChrom;
			nBits = nB;
			fitness = fit;
			mutProb = pMut;
		}

		// Getters and a setter
		public uint Chrom
		{
			get { return this.chrom; }
		}

		public float Fitness
		{
			get { return this.fitness; }
			set { this.fitness = value; }
		}

		// Mutates a random bit mutProb of the time
		public void Mutate ()
		{
			if (UnityEngine.Random.value < mutProb)
			{
				int mutPt = UnityEngine.Random.Range(0, nBits);	// 0 to nBits - 1
				int mutMask = 1 << mutPt;		// Build mask of 1 at mutation point
				chrom = chrom ^ (uint)mutMask;	// xor the mask, which flips that bit
			}
		}

		// Make it easier to write an Individual
		public override string ToString()
		{
			return (chrom + " " + fitness);
		}
	}
}
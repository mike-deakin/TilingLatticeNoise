//using System;
//
//namespace NoiseGeneration
//{
//	public class FeistelHasher
//	{
//
//		int m_seed_pow;
//
//		public FeistelHasher (int seed_power)
//		{
//			m_seed_pow = seed_power;
//		}
//
//		private int[] FeistelNHash(int sleft, int sright, int[] coord){
//			//Experiment:
//			//Using a feistel network to hash the coordinates with a seed that is set with each tile.
//			//Let seedL and seedR be  the "Plaintext" and the coordinates be the "round keys"
//
//			int rounds = coord.Length;
//			int L = sleft;
//			int R = sright;
//
//			for (int i = 0; i < rounds; i++) {
//				int temp = R;
//				R = L ^ FeistelRound (R, coord [i]);
//				L = temp;
//			}
//
//			return ;
//		}
//
//		private int FeistelRound(int right, int k){
//			//Round function to acompany the fiestel hash.
//			int kexp = KeyExpand(k); //First, expand k to n+1 bits.
//
//			return kexp;
//		}
//
//		private int KeyExpand(int k){
//			return ((((k * 25214903917) + 11) % (2 << 48)) >> 16) % (2 << (m_seed_pow + 1)); //Java linear congruential generator numbers then take mod 2^(m_seed_pow+1)
//		}
//	}
//}


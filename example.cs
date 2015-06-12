/* This file shows example usage of the NoiseGeneration library.
 *	This is just a simple example that shows the minimum components needed
 */

using System;
using NoiseGeneration;

class App
{
	public static void Main ()
	{
		//These delegates determine the behaviour of the algorithm.
		//The defaults below produce noise as in Perlin's improved noise algorithm
		//You can define custom functions instead of these in a new class
		//that inherits from interfaces IFader, ILerper or IProximity
		IFader fader = new PerlinQuinticFader();
		ILerper lerper = new BasicLerper();
		IProximity prox = new ProximityPerlinGradient();
		
		//Simply the number of dimensions you want to work in.
		//Perlin noise slows exponentially as the number of dimensions increases
		//and this implementation is very memory intensive at the moment,
		//so keep your "image" size as low as possible when working in 3 or more dimensions.
		int dimensions = 2;
		
		//These are used to determine the "image" height/width (in px)
		//and the height/width of the grid seeding the noise respectively.
		//For best results, the image width should be a power of 2 (2^n)
		//and the hash width should be a power of 2 plus 1 ((2^n) + 1)
		//and less than the image width.
		int imgSize = 256;
		int hashSize = 129;
		
		//These determine the properties of so called "fractal noise".
		//start_scale should be a power of 2 (2^n) smaller than hashSize and
		//defines the scale of the noise. Higher numbers give higher frequency noise
		//Layers determines how many iterations of noise are computed.
		//For each layer, the scale of noise is doubled.
		//The scale should not exceed the hash size,
		//so start_scale + 2^(layers - 1) should be less than hashSize.
		//For non-fractal noise, simply set layers to 1.
		int start_scale = 2;
		int layers = 4;
		
		//Also affects fractal noise. Each successive layer hash this much affect
		//on the final noise value at each pixel, relative to the previous step.
		//1 is equal effect, 0 is no effect. Values greater than 1 are legal,
		//and cause finer noise to be more dominant in the final image.
		//Coarser noise then don't have much visible effect above about 1.1 or 1.2
		//Negative values work too, but persistence is applied multiplicatively
		//so you get a grainy high contrast effect.
		float persistence = 0.5f;
		
		//Initialise the generator.
		LatticeNoiseGenerator lng = new LatticeNoiseGenerator(	fader, lerper, prox, //Delegate classes
																dimensions, imgSize, hashSize, //Tile size information
																start_scale, layers, persistence); //Fractal noise settings.
																
		
		//To generate a tile, simply give the generator a coordinate to generate at.
		lng.GenerateNoiseTile (3, 2); //x, y, z, etc.
		
		//To retrieve the generated noise data, retrieve the tile object
		//from the generator by giving the coordinate again...
		NoiseTile example_tile = lng.GetTile(3, 2);
		
		//... and access the juicy data inside.
		for (int x = 0; x < imgSize; x++){
			for (int y = 0; y < imgSize; y++){
				int pixel_value = example_tile.data.GetAt(x, y);
				DoSomethingWith(pixel_value);
			}
		}
	}
	
	public static void DoSomethingWith(NoiseTile tile)
	{
		//Console.WriteLine("You did a thing!");
	}
}

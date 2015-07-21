# TilingLatticeNoise
A tiling and non-repeating adaptation of Perlin's improved noise algorithm generalised to n dimensions.

For example usage, refer to example.cs.
Essentially, select your method of lerp and fade (interpolation) and the proximity function by instatiating their classes then initialise a LatticeNoiseGenerator object with these and the settings of your noise space (dimensions, size, detail, etc.).
The LatticeNoiseGenerator generates and has access to the generated noise tiles, so call GenerateNoiseTile(coordinates[]) to generate a noise tile and GetTile(coordinates[]) to access it.
Noise data is stored in an N-dimensional array (INDArray) in a NoiseTile object.

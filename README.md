# TilingLatticeNoise
#### A tiling and non-repeating adaptation of Perlin's improved noise algorithm generalised to n dimensions.

#### ¡WARNING!
  __*Noise generating algorithms generally take exponentially longer the bigger you make the target tile.*__
  
  __*This effect is especially true with this implementation, as this algorithm theoretically works in any number of dimensions, further increasing compute time the higher you go.*__
  
  __*It is not recomended you attempt to use this library (at least for Perlin Noise) in any more than 4 dimensions, and even then keep things as small as you can.*__


For example usage, refer to example.cs.

Most interaction happens with a `LatticeNoiseGenerator` object. Calling `GenerateNoiseTile(coordinate[])` on it will do exactly that if it can, and calling `GetTile(coordinate[])` will return a `NoiseTile` if one has already been made for that coordinate.

This project is intended to be customisable and extensible to any noise generating regime you care to imagine.
To create your own noise regime, extend as many of the interfaces as you need to and pass them into the constructor of your `LatticeNoiseGenerator`. Currently only Perlin noise has been implemented.

#### The Interfaces, and roughly what they do
  * `INDArray`
    
    An N-dimensional array object. This is a standard array member with helper methods to make treating it like an N-dimensional object more human.
  * `ILerper`
    
    Linear interpolation. You probably wont ever need any other implementation than the `BasicLerper`. Just here for completeness
  * `IFader`
    
    Contains a `Fade(t)` function that smooths or otherwise adjusts the result of linear interpolation. One simple way of changing how thenoise generator behaves.
  * `IProximity`
    
    Calculates 'distance' from the internal coordinate vector and the vector at the external grid points. Actually calculating euclidean distance gives 'value noise'. Perlin noise uses the gradient of the two vectors. This is the main way of generating an alternate noise regime and will give the most dramatic and interesting differences.

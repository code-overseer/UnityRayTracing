# RayTracing in Unity

Just having some fun with ray tracing in Unity. These are ran at quite low resolutions (720p) due to lack of GPU horsepower. 

## System Specification
Macbook Pro 2017 with AMD Radeon Pro 555 (2GB VRAM)

## Screenshots
![alt text](https://github.com/muramasa2402/UnityRayTracing/blob/master/Assets/Screenshots/screenshot01.png)

A scene with 4 spheres with a point light source: red transparent, colourless transparent, purple diffuse, green mirror. Implemented a simple hack to create the light focusing effect when it passes through transparent spheres causing the middle of the shadow to light up.

![alt text](https://github.com/muramasa2402/UnityRayTracing/blob/master/Assets/Screenshots/screenshot02.png)
Same scene but with a cube.

![alt text](https://github.com/muramasa2402/UnityRayTracing/blob/master/Assets/Screenshots/screenshot04.png)

![alt text](https://github.com/muramasa2402/UnityRayTracing/blob/master/Assets/Screenshots/screenshot03.png)
Same as example 2 but all objects have diffuse surfaces. Rendered using monte carlo integration to achieve soft shadows and colour bleeding with depth 3 and 128 samples per pixel.
# unity-image-rgb-to-xyz

This is an experimental project to map each pixel's RGB value to an object's XYZ coordinate.

### Information

There's not much else to it, really.

Included in the project are a few test images that demonstrate the functionality of the application. In order to process new images, these images need to be added to the scene and the project recompiled - there is no support for processing raw files, and no file browser.

Note that there is a memory leak and the code is not as optimised as it could be. This was purely a technical experiment to see how images look when mapped to 3D space.

For more detailed post, see my blog write up [here](https://programmerkb.wordpress.com/2017/06/17/colour-to-spatial-mapping-image-processor/).

### Controls

There are two cameras in scene - one that is animated (horribly, originally set up to capture GIFs), and another that allows for free-roaming. In order to switch cameras, enable the camera you want to use in the scene and disable the other.

**Left click**          - Cycle loaded image.

**Right click**         - Freelook (free-roam camera only)

**Right click + WASD**  - Move camera (free-roam camera only)

### Attribution

[GhostFreeRoamCamera](https://www.assetstore.unity3d.com/en/#!/content/19250) by Goosey Gamez, available on the Unity Asset Store.

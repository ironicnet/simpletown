---
layout: post
title: Building Mechanics
category: [mechanics, building]
excerpt: Hello World! This is the first post of this blog. Who am I? I'm a developer from Argentina, trying to get started into game developing. What is Simple Town? Simple town is meant to be a simple game to help me try and see what i can achieve with Unity.

---

# Building Mechanics #1

Right now the buildings are just a cube. The workers walk trought them with no problem.

One of the first things that i want to change, is that the workers can't walk through them.
Each building will have a "waypoint". From this waypoint the workers will enter from and exit to. Also all the materiales required by the building and produced by the building, will be dropped there.

Surely this waypoint will be a connection to roads or other buildings.

Also i want to have some kind of representation of the construction status. A transparency in the building should be easy to implement. In the future it could be an animation.

Each building should have a list of workers in it. A way to kick them out.

Also before building, it should check if it is pausible to build in that position.

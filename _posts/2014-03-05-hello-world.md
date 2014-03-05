---
layout: post
title: Hello World
category: Meta

excerpt: Hello World! Vestibulum imperdiet adipiscing arcu, quis aliquam dolor condimentum dapibus. Aliquam fermentum leo aliquet quam volutpat et molestie mauris mattis. Suspendisse semper consequat velit in suscipit.

---

Hello World!

This is the first post of this blog.

# Who am I?
I'm a developer from Argentina, trying to get started into game developing.

#What is Simple Town?

Simple town is meant to be a simple game to help me try and see what i can achieve with Unity.

So far, i'm not 'art' interested, but more into mechanics.

This game is inspired by the "old" game [The Settlers](http://en.wikipedia.org/wiki/The_Settlers "The Settlers").

#The Game
Right now, you start with your "Base", castle, admin building, whatever. 

You have 2 workers available. If you click somewhere and they are available (black), they will go in that direction. Once they are walking (green), you can't cancel it. So be click wise :P

If you right click on a worker it will show the buildings available to build:
 - H: A House. Building houses increments the amount of available workers. It costs 5 of wood.
 - W: A woodcutter. A woodcutter will look out for trees in the area and produce wood. It costs 5 of wood.

Whenever you build a worker will lay down the building "placeholder" and others will have to bring down the materials required to build it. Right now there is no way to cancel a building and to free the active worker. So keep a worker available whenever you can.

And that's it. I'll keep it improving.

##Collaborating
If you feel that you can help me out with anything (coding,art, ideas), let me know!.

#The Roadmap 
The "roadmap" is in the github issue tracking system.

But i will copy what i have today:

##Enhancements:
  
  - [#16](https://github.com/ironicnet/simpletown/issues/16): The buttons should be nicer :P 
  - [#15](https://github.com/ironicnet/simpletown/issues/15): When the workers carry a material, it should be shown in the unit. 
  - [#14](https://github.com/ironicnet/simpletown/issues/14): When the workers ends a task (build, carry material), they should check if they can help or they should return or where to go. 
  - [#12](https://github.com/ironicnet/simpletown/issues/12): The woodcutter should have a radius of action, and a wait time. 
  - [#11](https://github.com/ironicnet/simpletown/issues/11): The units (house and buildings) should have a way to be selected and the user can see some info about it. 
  - [#10](https://github.com/ironicnet/simpletown/issues/10): The builings should have a list of workers in it. 
  - [#9](https://github.com/ironicnet/simpletown/issues/9): The buildings need a "waypoint" where the workers could throw and pickup the resources. 
  - [#8](https://github.com/ironicnet/simpletown/issues/8): The workers should be move using double click instead of a single click.  
  - [#6](https://github.com/ironicnet/simpletown/issues/6): The trees should check if there isn't a building before "spawning". 

##Bugs:

  - [#4](https://github.com/ironicnet/simpletown/issues/4): The woodcutter doesn't produce wood or logs. 
  - [#3](https://github.com/ironicnet/simpletown/issues/3): The woodcutter shouldn't cut from inside the tree :P
  - [#2](https://github.com/ironicnet/simpletown/issues/2): The units when arrives and changes the new waypoint, they slow down and accelerates again. Very annoying. This is because i'm using the Vecto3.Lerp. I should try with the CharacterController.SimpleMove. 
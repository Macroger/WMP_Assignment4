# WMP_Assignment4

This program was the assignment for Windows and Mobile Programming, a course in the SET program I took at Conestoga College. This course was focused
heavily on the windows side of things and most of the assignments encouraged writing the solution in C# with or without WPF frontend (User Interface).

This assignment was designed to give the student further experience using threads and tasks in a WPF environment. The goal was to create a program that generates
lines which move around independently from each other using multi threading techniques. I utilized tasks for multi threading in this program and created a XAML
based WPF UI to control the spawning of lines, optional trails (an optional bonus objective of the assignment), and how fast they move across the screen.
I added buttons to pause and resume the worker threads for additional optional bonus marks, and they are fully functional. 

Probably the hardest part about this project for me was figuring out the proper methods to spawn and interact/control worker threads in tasks. 
Also, the boundary code was a pain as well, for detecting when the lines have hit the boundaty of their allowed zone. They are supposed to
bounce off the "walls" of their zone but it took awhile before I got that part correct. In the end I did achieve the desired result and I believe
it is actually sorta fun to watch the lines bounce around in random patterns. Adding the trails for visual effect is just icing on the cake.

I am proud of this work because it was the first times I utilized threads to such an extent. It also got a good grade from my professor - 100%.

This project was built on Visual Studio 2019 using .NET Framework targeting version 4.7.2.

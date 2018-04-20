# Inspiring.MVVM

This framework was developed for a large WPF business application with a **very** complex UI. The framework is still used but not actively developed anymore. Despite some minor stuff it is principally usable and stable, but unfortunately nobody ever found time and motivation to actually document it and prepare for public use.

Here are some of the goals and values that guided me in the architecture and design of the framework:
1.	Make it easy and intuitive to build WPF applications that have a clean architecture which value a clear distribution of concerns, layering and separation of concerns.
2.	Prefer compile-time checking over runtime-checking by using static typing, generics and designs that are checked by the compiler.
3.	Minimize the code required to be written by the developer but prefer a little more code that is straight-forward and hard to get wrong over a little less code that is more complex and harder to maintain.
4.	Make the API intuitive and easy to discover without resorting to the documentation (optimize for IntelliSense for example).
5.	Design the library so that it is
    - Easily understandable and maintainable.
    - Extensible and customizable in most aspects.
6.	Only add essential features that are needed by most applications to the library. Maximize the value/lines of code ratio. Provide feature that are not used by many applications as source files that can be included in the target application.
7.	Strive for a clean design and excellent quality


If you want to dig in, I would suggest the following starting points, some of which are not up-to-date anymore, but still better than nothing.

I would suggest starting with the documentation fragments I wrote back then which can be found at: https://github.com/InspiringCode/Inspiring.MVVM/tree/master/Docs The "MVVM Documentation.docx" gives a rough overview. "UI Framework.docx" says a little more about the screen concept. The "API Design" txt files show some more-or-less examples of how to create View-Models. For seeing some real code, I would suggest starting at https://github.com/InspiringCode/Inspiring.MVVM/tree/master/Source/MvvmTest/ApiTests where some features of the framework are demonstrated.

The ViewBinder concept might also be of interest, it provides a way to bind XAML elements in the code behind files to get compile time safety with XAML names. A very simplistic example is sketched here: https://github.com/InspiringCode/Inspiring.MVVM/blob/master/Source/MvvmTest/Views/ViewBinderTest.cs

If you have any questions do not hesitate to contact me :)

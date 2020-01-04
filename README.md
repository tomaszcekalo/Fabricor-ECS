## Moving to rust
I am moving my entire dev suite to rust and therefore this package is going to be archived. Feel free to fork it if you so desire.


# Fabricor-ECS
This is a c# ECS framework used in the Fabricor engine.



### Notes

* All entity id's are uint64
* Max entity size is about 65000 bytes.
* Max heap size is long.MaxValue bytes.
* In memory a entity is a ushort of how big it is followed by components.
* In memory a component has to start with a ushort of its size and then a uint representing the component type.
* In your custom component structs you must include a ComponentHeader struct at the top of the declaration.

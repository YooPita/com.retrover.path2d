# Retroever.Path2d
2d curved path movement project for Unity

This solution will allow you to build a path from bezier curves, then create a client and move it along the path.

⭐ The project has an OOP architecture, which makes it easy to integrate.

## How to start
1. Add an empty object to the scene. Add the "CurvedPath" script to it.
2. Create a curve using this component.
3. Create another object with the "ExamplePathClient" script. Specify in it the curve that you created before.
4. ???
5. Press play and have profit 😎

## Instruction
### Path
#### 1. Create a List of Curve Points
`var points = new List<CurvePoint>();`
#### 2. Fill it with dots
You need to specify only the right handle. The left handle is a reflection of the right handle.

`CurvePoint(Vector2 position, Vector2 rightHandle)`
#### 3. Create a path
`Path(List<CurvePoint> points, int precision, bool isLoop)`
- `precision` - how many points will be set on the curve. The curve consists of segments in order to simplify the search.
- `isLoop` - is the path closed.
### Client
#### 1. Create a client
Create a class and inherit from an interface `IPathClient`.

In method `UpdatePosition(PathPosition position)`, the client gets its position when it's updated. You need to use this position to transform your client.

`PathPosition` has get-fields:
- `Vector2 Position` - position in 2d space
- `NormalizedVector2 Normal` - forward direction
- `float Length` - length from the beginning of the path
#### 2. Communication with the path
For convenient interaction with the path, use `PathGrip`. Create an instance of this object and put it in a variable of type IPathGrip.

`PathGrip(IPathClient client, IPath path)` - Give him yourself and the path on which you want to move.
- To attach your client to the path, use method `Attach(Vector2 position)`
- Once a client has been attached to a path, it can be moved. To move the client forward by 1 pixel, pass `1f` to the following `Move(float movement)` method

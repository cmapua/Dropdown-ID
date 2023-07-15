# Dropdown ID

Converts an Integer inspector property into a dropdown list of IDs, using Odin Inspector. This tool was intended for use with database systems, where asset files can be identified by a custom ID property. This eliminates the need to manually input ID values in the inspector, greatly minimizing the risk of human error.

## Requirements
Developed and tested in Unity 2019.4.30 (LTS) with Odin Inspector 3.1.13.0.

## Usage

1. Create a new `ScriptableObject` that implements `IIdentifiable`.
```csharp
public class SampleIdentifiable : ScriptableObject, IIdentifiable
{
    [SerializeField]
    private int _id;

    public int ID => _id;
    public string DropdownOptionLabel => name;
}
```

2. Create a new attribute class deriving from `BaseIdentifiableAttribute`. This will be the attribute that will be used to associate with its corresponding `IIdentifiable` implementation.
```csharp
public class SampleIdentifiableAttribute : BaseIdentifiableAttribute { }
```

3. Create a new custom attribute drawer deriving from `BaseIdentifiablePropertyDrawer<TAttribute,TIdentifiable>`. `TAttribute` must be the attribute type created in step 2, and `TIdentifiable` must be the `IIdentifiable` implementation created in step 1. Place this class in an Editor-only assembly.

```csharp
public class SampleIdentifiablePropertyDrawer : BaseIdentifiablePropertyDrawer<SampleIdentifiableAttribute,SampleIdentifiable> 
{ 
    // optional: FindIdentifiables() can be overridden to customize how SampleIdentifiables are found in the project
    // (this is required for IIdentifiables that are not ScriptableObjects)
    protected override List<SampleIdentifiable> FindIdentifiables()
    {
        // ...
    }
}
```

4. The custom attribute is technically ready for use now, but the dropdown won't be populated until instances of `SampleIdentifiable` are created (and with unique `ID` values).
5. To use the custom attribute, decorate any `int` field or property with `[SampleIdentifiable]`.
```csharp
public class SampleUsage : MonoBehaviour
{
    [SampleIdentifiable]
    public int Id;
    
    // can also be used on collections of integers
    [SampleIdentifiable]
    public int[] Ids;

    private void Start()
    {
        Debug.Log($"Id is {Id}");
    }
}
```

## Disclaimer and Limitations
- `IIdentifiable` is intended to be implemented by `ScriptableObject`s. There is no guarantee it will work with other implementations (e.g. `MonoBehaviour`).
- If the serialized `ID` value is changed in the `IIdentifiable` asset, the dropdown of any properties referencing that ID will be affected (will either be set to 'None' since it cannot find the `IIdentifiable` with that ID, or will be set to a different `IIdentifiable` with matching ID). It is recommended to only use ID values that are set once and never change.
- If two or more `IIdentifiable`s have the same `ID` value, the selected dropdown option will display the label of the first matching `IIdentifiable`.
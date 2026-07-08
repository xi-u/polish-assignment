using System.Collections.Generic;

public class EntityList : Entity
{
    protected List<Entity> entities;

    public EntityList(string name) : base(name)
    {
        entities = new List<Entity>();
    }
}
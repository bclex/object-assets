using System.Collections.Generic;

namespace OA.Ultima.World.Maps
{
    /// <summary>
    /// Represents a single tile on the Ultima Online map.
    /// </summary>
    public class MapTile
    {
        List<AEntity> _entities;
        bool _needsSorting;
        List<StaticItem> _staticItemList = new List<StaticItem>();
        static List<Item> _itemsAtZList = new List<Item>();

        public MapTile()
        {
            _entities = new List<AEntity>();
        }

        /// <summary>
        /// The Ground entity for this tile. Every tile has one and only one ground entity.
        /// </summary>
        public Ground Ground { get; private set; }

        public int X
        {
            get { return Ground.Position.X; }
        }

        public int Y
        {
            get { return Ground.Position.Y; }
        }

        /// <summary>
        /// Adds the passed entity to this Tile's entity collection, and forces a resort of the entities on this tile.
        /// </summary>
        /// <param name="entity"></param>
        public void OnEnter(AEntity entity)
        {
            // only allow one ground object.
            if (entity is Ground)
            {
                if (Ground != null)
                    Ground.Dispose();
                Ground = (Ground)entity;
            }
            // if we are receiving a Item with the same position and itemID as a static item, then replace the static item.
            if (entity is Item)
            {
                var item = entity as Item;
                for (var i = 0; i < _entities.Count; i++)
                    if (_entities[i] is Item)
                    {
                        var comparison = _entities[i] as Item;
                        if (comparison.ItemID == item.ItemID && comparison.Z == item.Z)
                        {
                            _entities.RemoveAt(i);
                            i--;
                        }
                    }
            }
            _entities.Add(entity);
            _needsSorting = true;
        }

        public bool ItemExists(int itemID, int z)
        {
            for (var i = 0; i < _entities.Count; i++)
                if (_entities[i] is Item)
                {
                    var comparison = _entities[i] as Item;
                    if (comparison.ItemID == itemID && comparison.Z == z)
                        return true;
                }
            return false;
        }

        /// <summary>
        /// Removes the passed entity from this Tile's entity collection.
        /// </summary>
        /// <param name="entity"></param>
        public void OnExit(AEntity entity)
        {
            _entities.Remove(entity);
        }

        /// <summary>
        /// Checks if the specified z-height is under an item or a ground.
        /// </summary>
        /// <param name="z">The z value to check.</param>
        /// <param name="underEntity">Returns the first roof, surface, or wall that is over the specified z.
        ///                         If no such objects exist above the specified z, then returns null.</param>
        /// <param name="underGround">Returns the ground object of this tile if the specified z is under the ground.
        ///                         Returns null otherwise.</param>
        public bool IsZUnderEntityOrGround(int z, out AEntity underEntity, out AEntity underGround)
        {
            // getting the publicly exposed Entities collection will sort the entities if necessary.
            var entities = Entities;
            underEntity = null;
            underGround = null;
            for (var i = entities.Count - 1; i >= 0; i--)
            {
                if (entities[i].Z <= z)
                    continue;
                if (entities[i] is Item) // checks Item and StaticItem entities.
                {
                    var data = ((Item)entities[i]).ItemData;
                    if (data.IsRoof || data.IsSurface || (data.IsWall && data.IsImpassable))
                        if (underEntity == null || entities[i].Z < underEntity.Z)
                            underEntity = entities[i];
                }
                else if (entities[i] is Ground && entities[i].GetView().SortZ >= z + 12)
                    underGround = entities[i];
            }
            return underEntity != null || underGround != null;
        }

        /// <summary>
        /// Returns a list of static items in this tile. ONLY CALL ONCE AT A TIME. NOT THREAD SAFE.
        /// </summary>
        /// <returns></returns>
        public List<StaticItem> GetStatics()
        {
            var items = _staticItemList;
            _staticItemList.Clear();
            for (var i = 0; i < _entities.Count; i++)
                if (_entities[i] is StaticItem)
                    items.Add((StaticItem)_entities[i]);
            return items;
        }

        /// <summary>
        /// Returns a list of items at the specified z in this tile. ONLY CALL ONCE AT A TIME. NOT THREAD SAFE.
        /// </summary>
        /// <returns></returns>
        public List<Item> GetItemsBetweenZ(int z0, int z1)
        {
            var items = _itemsAtZList;
            _itemsAtZList.Clear();
            for (var i = 0; i < _entities.Count; i++)
                if (_entities[i] is Item && _entities[i].Z >= z0 && _entities[i].Z <= z1)
                    items.Add((Item)_entities[i]);
            return items;
        }

        private bool matchNames(ItemData m1, ItemData m2)
        {
            return (m1.Name == m2.Name);
        }

        private void InternalRemoveDuplicateEntities()
        {
            var itemsToRemove = new int[0x100];
            var removeIndex = 0;
            for (var i = 0; i < _entities.Count; i++)
            {
                // !!! TODO: I think this is wrong...
                for (var j = 0; j < removeIndex; j++)
                    if (itemsToRemove[j] == i)
                        continue;
                if (_entities[i] is StaticItem)
                {
                    // Make sure we don't double-add a static or replace an item with a static (like doors on multis)
                    for (var j = i + 1; j < _entities.Count; j++)
                        if (_entities[i].Z == _entities[j].Z)
                            if (_entities[j] is StaticItem && ((StaticItem)_entities[i]).ItemID == ((StaticItem)_entities[j]).ItemID)
                            {
                                itemsToRemove[removeIndex++] = i;
                                break;
                            }
                }
                else if (_entities[i] is Item)
                {
                    // if we are adding an item, replace existing statics with the same *name* We could use same *id*, but this is more robust for items that can open ...
                    // an open door will have a different id from a closed door, but the same name. Also, don't double add an item.
                    for (var j = i + 1; j < _entities.Count; j++)
                        if (_entities[i].Z == _entities[j].Z)
                            if ((_entities[j] is StaticItem && matchNames(((Item)_entities[i]).ItemData, ((StaticItem)_entities[j]).ItemData)) ||
                                (_entities[j] is Item && _entities[i].Serial == _entities[j].Serial))
                            {
                                itemsToRemove[removeIndex++] = j;
                                continue;
                            }
                }
            }
            for (var i = 0; i < removeIndex; i++)
                _entities.RemoveAt(itemsToRemove[i] - i);
        }

        public List<AEntity> Entities
        {
            get
            {
                if (_needsSorting)
                {
                    InternalRemoveDuplicateEntities();
                    TileSorter.Sort(_entities);
                    _needsSorting = false;
                }
                return _entities;
            }
        }

        public void ForceSort()
        {
            _needsSorting = true;
        }
    }
}

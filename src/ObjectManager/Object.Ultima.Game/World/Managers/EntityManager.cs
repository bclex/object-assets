using OA.Ultima.Data;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Entities.Items.Containers;
using OA.Ultima.World.Entities.Mobiles;
using System;
using System.Collections.Generic;

namespace OA.Ultima.World.Managers
{
    class EntityManager
    {
        WorldModel _model;
        Dictionary<int, AEntity> _entities = new Dictionary<int, AEntity>();
        List<AEntity> _entities_Queued = new List<AEntity>();
        List<Serial> _retainedPlayerEntities = new List<Serial>();
        bool _entitiesCollectionIsLocked;
        List<int> _serialsToRemove = new List<int>();
        List<OrphanedItem> _orphanedItems = new List<OrphanedItem>();

        public EntityManager(WorldModel model)
        {
            _model = model;
        }

        public void Reset(bool clearPlayerEntity = false)
        {
            _orphanedItems.Clear();
            _retainedPlayerEntities.Clear();
            if (!clearPlayerEntity)
            {
                var player = GetPlayerEntity();
                if (player != null)
                    RetainPlayerEntities(player, _retainedPlayerEntities);
            }
            foreach (var e in _entities.Values)
                if (!_retainedPlayerEntities.Contains(e.Serial))
                    e.Dispose();
        }

        void RetainPlayerEntities(Mobile player, List<Serial> retained)
        {
            retained.Add(player.Serial);
            for (var i = (int)EquipLayer.FirstValid; i <= (int)EquipLayer.LastUserValid; i++)
            {
                var e = player.Equipment[i];
                if (e != null && !e.IsDisposed)
                {
                    retained.Add(e.Serial);
                    if (e is ContainerItem)
                        RecursiveRetainPlayerEntities(e as ContainerItem, retained);
                }
            }
        }

        void RecursiveRetainPlayerEntities(ContainerItem container, List<Serial> retained)
        {
            foreach (var e in container.Contents)
                if (e != null && !e.IsDisposed)
                {
                    retained.Add(e.Serial);
                    if (e is ContainerItem)
                        RecursiveRetainPlayerEntities(e as ContainerItem, retained);
                }
        }

        public Mobile GetPlayerEntity()
        {
            // This could be cached to save time.
            if (_entities.ContainsKey(WorldModel.PlayerSerial))
                return (Mobile)_entities[WorldModel.PlayerSerial];
            return null;
        }

        public void Update(double frameMS)
        {
            if (WorldModel.IsInWorld)
                UpdateEntities(frameMS);
        }

        void UpdateEntities(double frameMS)
        {
            // redirect any new entities to a queue while we are enumerating the collection.
            _entitiesCollectionIsLocked = true;
            var player = GetPlayerEntity();
            if (player == null) { } // wait for the server to send us an updated player entity.
            else
            {
                // Update all other entities, disposing when they are out of range.
                var player_position = player.DestinationPosition;
                foreach (var entity in _entities)
                {
                    entity.Value.Update(frameMS);
                    if (Math.Abs(player_position.X - entity.Value.Position.X) > (entity.Value.GetMaxUpdateRange()) ||
                        Math.Abs(player_position.Y - entity.Value.Position.Y) > (entity.Value.GetMaxUpdateRange()))
                        entity.Value.Dispose();
                    if (entity.Value.IsDisposed)
                        _serialsToRemove.Add(entity.Key);
                }
            }
            // Remove disposed entities
            foreach (var i in _serialsToRemove)
                _entities.Remove(i);
            _serialsToRemove.Clear();
            // stop redirecting new entities to the queue and add any queued entities to the main entity collection.
            _entitiesCollectionIsLocked = false;
            foreach (var e in _entities_Queued)
                _entities.Add(e.Serial, e);
            _entities_Queued.Clear();
        }

        public Overhead AddOverhead(MessageTypes msgType, Serial serial, string text, int fontID, int hue, bool asUnicode)
        {
            if (_entities.ContainsKey(serial))
            {
                var ownerEntity = _entities[serial];
                var overhead = ownerEntity.AddOverhead(msgType, text, fontID, hue, asUnicode);
                return overhead;
            }
            return null;
        }

        public T GetObject<T>(Serial serial, bool create) where T : AEntity
        {
            T entity;
            // Check for existence in the collection.
            if (_entities.ContainsKey(serial))
            {
                // This object is in the m_entities collection. If it is being disposed, then we should complete disposal
                // of the object and then return a new object. If it is not being disposed, return the object in the collection.
                entity = (T)_entities[serial];
                if (entity.IsDisposed)
                {
                    _entities.Remove(serial);
                    if (create)
                    {
                        entity = InternalCreateEntity<T>(serial);
                        return (T)entity;
                    }
                    return null;
                }
                return (T)_entities[serial];
            }
            // No object with this Serial is in the collection. So we create a new one and return that, and hope that the server
            // will fill us in on the details of this object soon.
            if (create)
            {
                entity = InternalCreateEntity<T>(serial);
                return (T)entity;
            }
            return null;
        }

        T InternalCreateEntity<T>(Serial serial) where T : AEntity
        {
            var ctor = typeof(T).GetConstructor(new[] { typeof(Serial), typeof(Map) });
            var e = (T)ctor.Invoke(new object[] { serial, _model.Map });
            if (e.Serial == WorldModel.PlayerSerial)
                e.IsClientEntity = true;
            if (e is Mobile)
                for (var i = 0; i < _orphanedItems.Count; i++)
                    if (_orphanedItems[i].ParentSerial == serial)
                    {
                        (e as Mobile).WearItem(_orphanedItems[i].Item, _orphanedItems[i].Layer);
                        _orphanedItems.RemoveAt(i--);
                    }
            // If the entities collection is locked, add the new entity to the queue. Otherwise 
            // add it directly to the main entity collection.
            if (_entitiesCollectionIsLocked)  _entities_Queued.Add(e);
            else _entities.Add(e.Serial, e);
            return (T)e;
        }

        public void RemoveEntity(Serial serial)
        {
            if (_entities.ContainsKey(serial))
                _entities[serial].Dispose();
        }

        public void AddWornItem(Item item, byte layer, Serial parent)
        {
            var m = WorldModel.Entities.GetObject<Mobile>(parent, false);
            if (m != null)
                m.WearItem(item, layer);
            else
                _orphanedItems.Add(new OrphanedItem(item, layer, parent));
        }

        struct OrphanedItem
        {
            public readonly byte Layer;
            public readonly Item Item;
            public readonly Serial ParentSerial;

            public OrphanedItem(Item item, byte layer, Serial parent)
            {
                Item = item;
                Layer = layer;
                ParentSerial = parent;
            }
        }
    }
}


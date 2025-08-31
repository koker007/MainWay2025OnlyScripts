using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Game.Data;
using Game.Data.Serializable;
using UnityEngine;

namespace Game.Data.Block
{
    public abstract class BlockData: ICanSave<BlockData>
    {
        public string name;
        public string mod;
        public int variant;
        public Type type;

        /// <summary>
        /// Внешний вид
        /// </summary>
        //прозрачность
        TypeBlockTransparent transparentType = TypeBlockTransparent.NoTransparent;
        float transparentPower = 0.5f;
        /// <summary>
        /// Обший цвет блока
        /// </summary>
        protected Color color;

        //Рисовать соседские стены или нет
        public bool drawNeighbourWall = false;

        //Светимость
        public int lighting = 0;

        public BlockPhysics physics;

        public BlockData(BlockData blockData)
        {
            mod = blockData.mod;
            name = blockData.name;
            variant = blockData.variant;

            transparentType = blockData.transparentType;
            transparentPower = blockData.transparentPower;

            drawNeighbourWall = blockData.drawNeighbourWall;

            lighting = blockData.lighting;

            physics = blockData.physics;

        }

        public abstract Color GetColor();
        public abstract void SaveData();
        public string GetPathBlock() => $"{StrC.FOLDER_NAME_MOD}/{mod}/{StrC.FOLDER_NAME_BLOCKS}/{name}";
        public string GetPathVariation() => $"{GetPathBlock()}/{variant}";

        public enum Type
        {
            block = 0,
            voxels = 1,
            liquid = 2
        }

        //Получить меш куба на основе того какие стены нужно отрисовать
        public virtual Mesh GetMesh(bool face, bool back, bool left, bool right, bool up, bool down, Mesh mesh)
        {
            mesh ??= new Mesh();
            return mesh;
        }

        public BlockData()
        {
            //Инициализация блока по умолчанию
            physics = new BlockPhysics();
            physics.parameters = new BlockPhysics.Parameters();
        }

        public void LoadData()
        {
            throw new System.NotImplementedException();
        }
    }

    public class BlockPhysics
    {

        public ColliderZone[] zones;
        public Light light;
        public Parameters parameters;

        [System.Serializable]
        public class ColliderZone
        {
            public Vector3S pos; //Позиция старта
            public Vector3S size; //Размер относительно старта
        }
        public class Light
        {
            float lightRange;
        }
        public class Parameters
        {
            float viscosity = 1; //Вязкость

        }

        public ColliderZone[] loadColliderZone(string pathPhysics)
        {

            ColliderZone[] colliderZones = null;

            string pathFileColliders = pathPhysics + '/' + StrC.collidersZone;

            //Если файла нет
            if (!File.Exists(pathFileColliders))
                return colliderZones;

            //Если файл есть - загружаем
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(pathFileColliders, FileMode.Open);
            colliderZones = (ColliderZone[])bf.Deserialize(fileStream);
            fileStream.Close();

            zones = colliderZones;



            return colliderZones;

        }
        public void saveColliderZone(string pathPhysics)
        {
            string pathFileColliders = pathPhysics + '/' + StrC.collidersZone;

            //Если файл есть - удаляем
            if (File.Exists(pathFileColliders))
                File.Delete(pathFileColliders);

            //Если создавать файл не надо выходим
            if (zones == null)
                return;

            if (!Directory.Exists(pathPhysics))
            {
                Directory.CreateDirectory(pathPhysics);
            }


            //Создаем файл
            BinaryFormatter bf = new BinaryFormatter();
            FileStream collidersZoneStream = File.OpenWrite(pathFileColliders);
            bf.Serialize(collidersZoneStream, zones);
            collidersZoneStream.Close();
        }


    }

    public enum TypeBlockTransparent
    {
        NoTransparent = 0,
        CutOff = 1,
        Alpha = 2
    }
}

using ImageRecognition.Common;
using ImageRecognition.ImageRecognizing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageRecognition.Database
{
    public class JsonImagesDatabase : INormalizedImagesDatabase
    {
        private class DatabaseImage
        {
            public int Id { get; set; }
            public string ImageDescription { get; set; }
            public Axis MainInertiaAxis { get; set; }
        }

        public const Formatting JsonFormatting = Formatting.Indented;
        private readonly string databaseFilePath;
        private readonly string databaseFolderPath;
        private readonly Dictionary<int, DatabaseImage> storage;
        private int nextId;

        public JsonImagesDatabase(string databaseFolderPath)
        {
            if (!Directory.Exists(databaseFolderPath))
            {
                Directory.CreateDirectory(databaseFolderPath);
            }

            this.databaseFolderPath = databaseFolderPath;
            this.databaseFilePath = Path.Combine(this.databaseFolderPath, "database.json");
            this.storage = new Dictionary<int, DatabaseImage>();

            this.Load();
        }

        private IEnumerable<DatabaseImage> DatabaseImages
        {
            get
            {
                foreach (DatabaseImage image in storage.Values)
                {
                    yield return image;
                }
            }
        }

        public IEnumerable<NormalizedImage> Images
        {
            get 
            {
                foreach (DatabaseImage databaseImage in this.storage.Values)
                {
                    yield return this.CreateNormalizedImage(databaseImage);
                }
            }
        }

        public NormalizedImage AddImage(NormalizedImageInfo imageInfo)
        {
            DatabaseImage image = new DatabaseImage()
            {
                Id = this.nextId++,
                MainInertiaAxis = imageInfo.MainInertiaAxis,
                ImageDescription = imageInfo.ImageDescription
            };
            
            imageInfo.ImageSource.SavePng(File.OpenWrite(this.GetImagePath(image.Id)));
            this.AddImage(image);
            this.Save();

            return this.CreateNormalizedImage(image);
        }

        public NormalizedImage GetImage(int id)
        {
            return this.CreateNormalizedImage(this.storage[id]);
        }

        public void RemoveImage(int id)
        {
            File.Delete(this.GetImagePath(id));
            this.storage.Remove(id);
            this.Save();
        }

        private void Load()
        {
            this.nextId = -1;

            if (File.Exists(this.databaseFilePath))
            {
                string json = File.ReadAllText(this.databaseFilePath);
                IEnumerable<DatabaseImage> images = JsonConvert.DeserializeObject<IEnumerable<DatabaseImage>>(json) ?? Enumerable.Empty<DatabaseImage>();

                foreach (DatabaseImage image in images)
                {
                    this.AddImage(image);
                    this.nextId = Math.Max(this.nextId, image.Id);
                }
            }

            this.nextId++;
        }

        private void Save()
        {
            string json = JsonConvert.SerializeObject(this.DatabaseImages, JsonImagesDatabase.JsonFormatting);
            File.WriteAllText(this.databaseFilePath, json);
        }

        private NormalizedImage CreateNormalizedImage(DatabaseImage databaseImage)
        {
            return new NormalizedImage()
            {
                Id = databaseImage.Id,
                ImageDescription = databaseImage.ImageDescription,
                MainInertiaAxis = databaseImage.MainInertiaAxis,
                ImageStream = File.OpenRead(this.GetImagePath(databaseImage.Id)).ToMemoryStream()
            };
        }

        private string GetImagePath(int id)
        {
            string path = Path.Combine(this.databaseFolderPath, string.Format("{0}.png", id));

            return path;
        }

        private void AddImage(DatabaseImage image)
        {
            this.storage.Add(image.Id, image);
        }
    }
}

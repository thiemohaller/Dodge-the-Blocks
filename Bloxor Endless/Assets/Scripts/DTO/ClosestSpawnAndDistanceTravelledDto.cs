using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DTO {
    public class ClosestSpawnAndDistanceTravelledDto {
        public GameObject ClosestSpawn { get; set; }
        public float DistanceTraveledByPlayer { get; set; }
        public Vector3 PreviousPlayerPosition { get; set; }
        public float DistanceBetweenSpawnAndPlayer { get; set; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Player;

namespace Tips
{
    public class TipArrow : MonoBehaviour
    {
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private float showTime = 10f;

        private GameObject arrow;

        private float timeToHide;
        private Transform playerT;

        public static TipArrow instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        void Start()
        {
            Movement player = GameObject.FindObjectOfType<Movement>();
            if (player != null)
                playerT = player.transform;

            arrow = Instantiate(arrowPrefab);
            arrow.transform.parent = transform;
            arrow.SetActive(false);
        }

        void Update()
        {
            if (arrow.activeSelf)
            {
                Vector3 dir = (playerT.position - arrow.transform.position).normalized;
                dir.y = 90;
                arrow.transform.forward = dir;

                timeToHide -= Time.deltaTime;
                if (timeToHide <= 0f)
                    arrow.SetActive(false);
            }
        }

        public void ShowArrowAt(Vector3 pos)
        {
            arrow.transform.position = pos + Vector3.up * 1f;
            arrow.SetActive(true);

            timeToHide = showTime;
        }
    }
}
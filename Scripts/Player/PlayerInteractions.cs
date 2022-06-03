using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UI.Gameplay.Tips;
using Audio;

namespace Player
{
    public class PlayerInteractions : MonoBehaviour
    {
        [Header("InteractableInfo")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float sphereCastRadius = 0.1f;
        [SerializeField] [Range(0f, 1f)] private float toolsVolume = 0.5f;
        [SerializeField] private float toolsDistanceScale = 3f;
        [SerializeField] private LookableObjects lookObject;

        [Header("Pickup")]
        [SerializeField] private Transform hand;
        [SerializeField] private float maxDistance = 5f;
        [SerializeField] [Range(0.01f, 10f)] private float pickUpForce = 3f;
        [SerializeField] private int itemHoldLayer = 7;
        [SerializeField] private MoveableObject currentlyPickedUpObject;

        [Header("Drop")]
        [SerializeField] private float dropMaxForce = 8f;
        [SerializeField] private float dropMaxHoldTime = 4f;


        private bool mouseHold = false;


        private bool isCleaning = false;
        private bool isDipTool = false;
        public bool CanMove => !isCleaning && !isDipTool;

        private Dirty cleaingObject = null;
        private int oldItemLayer;

        private void Update()
        {
            UpdateLookObject();

            UpdatePickedUpObjectPostion();

            UpdateToolTips();

            if (isCleaning)
                CleaningDirty();
        }

        public void ReciveInput(bool mouseHold) => this.mouseHold = mouseHold;

        private void UpdateLookObject()
        {
            //Here we check if we're currently looking at an interactable object
            Vector3 raycastPos = playerCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            LookableObjects tLookObject = null;
            int layerMask = 1 << LookableObjects.layer & ~(1 << itemHoldLayer);
            if (Physics.SphereCast(raycastPos, sphereCastRadius, playerCamera.transform.forward, out RaycastHit hit, maxDistance, layerMask))
            {
                GameObject go = hit.collider.gameObject;
                if (go.TryGetComponent(out LookableObjects lookable))
                    tLookObject = lookable;
            }


            if (lookObject != tLookObject)
            {
                // changed look lookObject
                SetActive(lookObject, false);
                SetActive(tLookObject, true);

                lookObject = tLookObject;

                void SetActive(LookableObjects lookable, bool state)
                {
                    if (lookable != null && !(lookable is PlacingPlace))
                    {
                        MaterialPropertyBlock props = new MaterialPropertyBlock();
                        Renderer[] rends = lookable.gameObject.GetComponentsInChildren<Renderer>();
                        if (rends == null || rends.Length == 0)
                        {
                            Debug.LogWarning("can't set object: " + gameObject.name + " as active! Object has no Renderer component!");
                        }
                        else
                        {
                            foreach (Renderer rend in rends)
                            {
                                rend.GetPropertyBlock(props);
                                props.SetFloat("_Is_selection_on", state ? 1 : 0);
                                rend.SetPropertyBlock(props);
                            }
                        }
                    }
                }
            }
        }

        private void UpdatePickedUpObjectPostion()
        {
            if (currentlyPickedUpObject != null && !isDipTool)
            {
                currentlyPickedUpObject.transform.position = Vector3.Lerp(currentlyPickedUpObject.transform.position, hand.position, pickUpForce / 10);
                currentlyPickedUpObject.transform.rotation = Quaternion.Lerp(currentlyPickedUpObject.transform.rotation, hand.rotation, pickUpForce / 10);

                if (Vector3.Distance(currentlyPickedUpObject.transform.position, playerCamera.transform.position) < hand.localPosition.z)
                    currentlyPickedUpObject.transform.position = Vector3.Lerp(currentlyPickedUpObject.transform.position, hand.position, 0.03f);
            }
        }

        private void UpdateToolTips()
        {
            if (UIGameplayTips.instance != null)
            {
                // tips
                bool value;

                value = lookObject != null && lookObject is MoveableObject
                    && currentlyPickedUpObject == null;
                UIGameplayTips.instance.SetTip(UIGameplayTips.Tip.PickItem, value);

                value = currentlyPickedUpObject != null;
                UIGameplayTips.instance.SetTip(UIGameplayTips.Tip.HoldPutBack, value);

                value = currentlyPickedUpObject != null
                    && currentlyPickedUpObject is MoveableObject moveableObject && moveableObject.CanPitch;
                UIGameplayTips.instance.SetTip(UIGameplayTips.Tip.HoldThrow, value);

                value = lookObject != null && lookObject is Dirty
                    ||
                    lookObject != null && lookObject is Bucket bucket && bucket.CanUse
                    && currentlyPickedUpObject != null && currentlyPickedUpObject is Mopp
                    ;
                UIGameplayTips.instance.SetTip(UIGameplayTips.Tip.HoldClean, value);

                value = currentlyPickedUpObject != null && currentlyPickedUpObject is ObjectToPutBack;
                UIGameplayTips.instance.SetPlaceItemInHighlitedPlace(value);

                // proges bar
                if (isCleaning && cleaingObject != null)
                    UIGameplayTips.instance.SetHoldBar(cleaingObject.CleanPercent, cleaingObject.TimeTolCleanUp);
                else
                    UIGameplayTips.instance.SetHoldBar(0f, 0f);
            }
        }

        public void InteractWithLookObject()
        {
            if (isDipTool)
                return;

            
            // cleaning dirty
            if (lookObject != null && lookObject is Dirty dirty)
            {
                if (currentlyPickedUpObject == null)
                {
                    dirty.ShowToolTip();
                    return;
                }
                else if (currentlyPickedUpObject is Tool tool)
                {
                    if (!dirty.CanCleanBy(currentlyPickedUpObject))
                        dirty.ShowToolTip();
                    else
                    {
                        if (!tool.CanUse)
                            tool.ShowToolTip();
                        else
                            StartCleanIngDirty(dirty);
                    }
                    return;
                }
            }


            // clean mopp
            if (lookObject != null && lookObject is Bucket bucket
                    && currentlyPickedUpObject != null && currentlyPickedUpObject is Mopp mopp)
            {
                if (bucket.CanUse)
                {
                    mopp.CleanUp();
                    StopCoroutine(nameof(DipCurentPickedUpTool));
                    StartCoroutine(DipCurentPickedUpTool(mopp.gameObject, bucket.gameObject, 10f, 2f, 1f, bucket.UsingSound));
                }
                else
                {
                    bucket.ShowToolTip();
                }
                return;
            }


            // put to target place
            if (currentlyPickedUpObject != null && currentlyPickedUpObject is ObjectToPutBack objectToPutBack
                && lookObject != null && lookObject is PlacingPlace placingPlace)
            {
                GameObject go = currentlyPickedUpObject.gameObject;
                DropObject(0);
                StartCoroutine(SendObjectTo(go, placingPlace.GetDropPosition(objectToPutBack), 10));
                return;
            }


            // change Picked Up Object
            if (currentlyPickedUpObject == null)
            {
                if (lookObject != null && lookObject is MoveableObject moveable)
                    PickUpObject(moveable);
            }
            else
            {
                if (currentlyPickedUpObject is MoveableObject moveable && moveable.CanPitch)
                    StartCoroutine(nameof(ShootHoldItem));
                else
                    DropObject();
            }
        }

        #region -Cleaning-

        private void CleaningDirty()
        {
            if (mouseHold)
            {
                if (currentlyPickedUpObject is Mopp mopp)
                    mopp.Use(Time.deltaTime);

                if (cleaingObject == null)
                {
                    // finish cleaning
                    StopCleaningDirty();
                }
                return;
            }
            else
            {
                // interrupt cleaning
                StopCleaningDirty();
            }
        }

        private void StartCleanIngDirty(Dirty dirty)
        {
            if (AudioManager.instance != null)
            {
                AudioClip clip = (currentlyPickedUpObject as Tool).UsingSound;
                AudioManager.instance.PlaySound(new Sound().ObjectSound(clip, toolsVolume, toolsDistanceScale, gameObject).Loop(60));
            }

            mouseHold = true;

            cleaingObject = dirty;
            cleaingObject.StartClening();
            isCleaning = true;

            StartCoroutine(AnimCleaning(dirty.transform.position));
        }

        private void StopCleaningDirty()
        {
            if (AudioManager.instance != null)
            {
                AudioClip clip = (currentlyPickedUpObject as Tool).UsingSound;
                AudioManager.instance.StopSound(clip, gameObject);
            }

            hand.localRotation = Quaternion.Euler(0, 0, 0);
            if (cleaingObject != null)
                cleaingObject.StoptClening();
            cleaingObject = null;
            isCleaning = false;
        }

        private IEnumerator AnimCleaning(Vector3 dirtyPos)
        {
            hand.rotation = Quaternion.LookRotation(dirtyPos - hand.position);
            Vector3 rot = hand.rotation.eulerAngles;
            float xRotation = rot.x - 90;
            hand.rotation = Quaternion.Euler(xRotation, rot.y, rot.z);

            float speed = 60;
            float maxDegree = 15;
            while (isCleaning)
            {
                float degree = 0;
                while (isCleaning && degree < maxDegree)
                {
                    float move = speed * Time.deltaTime;
                    degree += move;
                    hand.Rotate(Vector3.forward, move);
                    yield return null;
                }
                degree = 0;
                while (isCleaning && degree < maxDegree)
                {
                    float move = speed * Time.deltaTime;
                    degree += move;
                    hand.Rotate(Vector3.back, move);
                    yield return null;
                } 
            }

            hand.localRotation = Quaternion.Euler(0, 0, 0);
        }

        #endregion

        private void PickUpObject(MoveableObject movable)
        {
            currentlyPickedUpObject = movable;
            oldItemLayer = currentlyPickedUpObject.gameObject.layer;
            SetLayerRecursively(currentlyPickedUpObject.gameObject, itemHoldLayer);

            currentlyPickedUpObject.GetComponent<Collider>().isTrigger = true;
            currentlyPickedUpObject.GetComponent<Rigidbody>().isKinematic = true;


            if (currentlyPickedUpObject is ObjectToPutBack toPutUp)
                toPutUp.PlaceToPut.GetComponent<PlacingPlace>().SetVisActive(true);
        }

        private IEnumerator ShootHoldItem() 
        {
            float addF = dropMaxForce / dropMaxHoldTime;
            float force = 0;
            yield return null;
            while (mouseHold && !isCleaning)
            {
                force += addF * Time.deltaTime;
                UIGameplayTips.instance.SetHoldBarThrow(Mathf.Min(force / dropMaxForce, 1));
                yield return null;
            }

            if (!isCleaning)
            {
                DropObject(force);
            }

            UIGameplayTips.instance.SetHoldBarThrow(0);
        }

        private void DropObject(float force = 0)
        {
            if (currentlyPickedUpObject == null)
                return;

            SetLayerRecursively(currentlyPickedUpObject.gameObject, oldItemLayer);

            currentlyPickedUpObject.GetComponent<Collider>().isTrigger = false;
            Rigidbody rigidbody = currentlyPickedUpObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;

            if (currentlyPickedUpObject.CanPitch)
            {
                force = Math.Clamp(force, 0, dropMaxForce);
                rigidbody.AddForce(playerCamera.transform.forward * force, ForceMode.Impulse);
            }


            if (currentlyPickedUpObject is ObjectToPutBack objectToPutBack)
                objectToPutBack.PlaceToPut.GetComponent<PlacingPlace>().SetVisActive(false);

            currentlyPickedUpObject = null;
        }

        private void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj)
            {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        private IEnumerator SendObjectTo(GameObject obj, Vector3 targetPos, float speed)
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            while (Vector3.Distance(obj.transform.position, targetPos) > 0.2f)
            {
                obj.transform.position = Vector3.Lerp(obj.transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }

            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
        }

        private IEnumerator DipCurentPickedUpTool(GameObject tool, GameObject targetObject, float speed, float hight, float deep, AudioClip clip)
        {
            isDipTool = true;
            int layer = tool.layer;
            SetLayerRecursively(tool, 0);
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySound(new Sound().ObjectSound(clip, toolsVolume, toolsDistanceScale, gameObject).Loop(60));

            Vector3 targetPos = targetObject.transform.position + Vector3.up * hight;
            while (Vector3.Distance(tool.transform.position, targetPos) > 0.05f)
            {
                tool.transform.position = Vector3.Lerp(tool.transform.position, targetPos, speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            targetPos = targetObject.transform.position + Vector3.up * (hight - deep);
            while (Vector3.Distance(tool.transform.position, targetPos) > 0.05f)
            {
                tool.transform.position = Vector3.Lerp(tool.transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }

            targetPos = targetObject.transform.position + Vector3.up * hight;
            while (Vector3.Distance(tool.transform.position, targetPos) > 0.05f)
            {
                tool.transform.position = Vector3.Lerp(tool.transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }

            isDipTool = false;
            SetLayerRecursively(tool, layer);
            if (AudioManager.instance != null)
                AudioManager.instance.StopSound(clip, gameObject);
        }
    }
}
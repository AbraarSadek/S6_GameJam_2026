using System;
using UnityEngine;
using KBCore.Refs;

public class Sight: Sense
    {
        public int FieldOfView = 45;
        public int ViewDistance = 100;
        private Transform playerTransform;
        private Vector3 rayDirection;
        
        // Callback for when player is detected
        public bool playerDetected = false;
        
        // Callback function when detected
        private Action onPlayerDetected;
        private Action onPlayerLost;
        
        protected override void Initialize()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        protected override void UpdateSense()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= detectionRate)
            {
                DetectAspect();
                elapsedTime = 0.0f;
            }
        }
        
        private void DetectAspect()
        {
            RaycastHit hit;
            rayDirection = playerTransform.position - transform.position;
            if (Vector3.Angle(rayDirection, transform.forward) < FieldOfView)
            {
                if (Physics.Raycast(transform.position, rayDirection, out hit, ViewDistance, ~LayerMask.GetMask("Ignore Raycast")))
                {
                    Aspect aspect = hit.collider.GetComponent<Aspect>();
                    if (aspect != null && aspect.affiliation == targetAffiliation)
                    {
                        Debug.Log("Enemy Detected");
                        if (!playerDetected) onPlayerDetected?.Invoke();
                        playerDetected = true;
                    }
                    else
                    {
                        if (playerDetected) onPlayerLost?.Invoke();
                        playerDetected = false;
                    }
                }
                else
                {
                    if (playerDetected) onPlayerLost?.Invoke();
                    playerDetected = false;
                }
            }
            else
            {
                if (playerDetected) onPlayerLost?.Invoke();
                playerDetected = false;
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isEditor || playerTransform == null)
                return;
            Debug.DrawLine(transform.position, playerTransform.position, Color.red);
            Vector3 frontRayPoint = transform.position + (transform.forward * ViewDistance);
            Vector3 leftRayPoint = Quaternion.Euler(0, FieldOfView, 0) * frontRayPoint;
            Vector3 rightRayPoint = Quaternion.Euler(0, -FieldOfView, 0) * frontRayPoint;
            Debug.DrawLine(transform.position, frontRayPoint, Color.green);
            Debug.DrawLine(transform.position, leftRayPoint, Color.green);
            Debug.DrawLine(transform.position, rightRayPoint, Color.green);
        }
        
        public void SetOnPlayerDetected(Action callback)
        {
            onPlayerDetected = callback;
        }

        public void SetOnPlayerLost(Action callback)
        {
            onPlayerLost = callback;
        }
        
        // void OnValidate() => this.ValidateRefs();
    }

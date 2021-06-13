using UnityEngine;

namespace DropCamTweak
{
    public class HiddenCameraScript : MonoBehaviour
    {
        private Quaternion rotationEulerAngles;
        private Quaternion prevRotationEulerAngles;

        private Vector3 position;
        private Vector3 prevPosition;

        private bool followPlayer = false;
        private bool playerFollow = false;
        private Camera camera;
        private bool dynamicFov;

        private float targetDistance;
        private float minDistance = .25f;
        private float maxDistance = 45f;
        private float targetHeight = 2f;
        private float lookHeight = 0.6118473f;

        private GameObject targetObject;
        private Vector3 targetPosition;
        private Vector3 targetLookPosition;
        private Vector3 headPosition;

        private const float DefaultFieldOfView = 80f;

        private float FieldOfView
        {
            get => fieldOfView;
            set
            {
                fieldOfView = Mathf.Clamp(value, minFieldOfView, maxFieldOfView);
                zoom = (fieldOfView - maxFieldOfView) / (minFieldOfView - maxFieldOfView);
            }
        }

        private float initalHeightOffset = 0f;

        public float Zoom
        {
            get => zoom;
            set
            {
                zoom = Mathf.Clamp01(value);
                fieldOfView = Mathf.Lerp(maxFieldOfView, minFieldOfView, zoom);
            }
        }

        private float zoom = 0.5f;

        private float zoomStep = 0.01f;
        private float maxFieldOfView = 120;
        private float minFieldOfView = 1;

        private float fieldOfView = DefaultFieldOfView;
        // private float fieldOfViewOffset = DefaultFieldOfView;

        private Light pointLight;

        private void OnEnable()
        {
            if (pointLight == null)
                pointLight = gameObject.AddComponent<Light>();
            pointLight.type = LightType.Point;

            pointLight.color = Color.black;


            camera = GetComponent<Camera>();
            targetObject = DropCamTweak.PlayerObject;
            FieldOfView = DropCamTweak.MainCamera.fieldOfView;
            initalHeightOffset = transform.position.y - transform.position.y;

            if (Input.GetKey(KeyCode.RightShift)) followPlayer = true;
            if (Input.GetKey(KeyCode.RightControl)) playerFollow = true;
            if (Input.GetKey(KeyCode.RightAlt)) dynamicFov = true;
            if (Input.GetKey(KeyCode.Menu)) pointLight.color = DropCamTweak.GoldColor;
        }

        private void FixedUpdate()
        {
            lookHeight = (lookHeight * 59f + headPosition.y - targetPosition.y) / 60f;
            if (Input.GetKey(KeyCode.Mouse3) || Input.GetKey(KeyCode.Mouse4))
            {
                if (Input.GetKey(KeyCode.Mouse3)) Zoom -= zoomStep;
                if (Input.GetKey(KeyCode.Mouse4)) Zoom += zoomStep;
                dynamicFov = false;
            }
        }

        private void Update()
        {
            rotationEulerAngles = transform.rotation;
            if (rotationEulerAngles != prevRotationEulerAngles)
                OnRotation();
            prevRotationEulerAngles = rotationEulerAngles;

            position = transform.position;
            if (position != prevPosition)
                OnMove();
            prevPosition = position;

            targetPosition = targetObject.transform.position;
            headPosition = DropCamTweak.PlayerHeadObject.transform.position;
            targetDistance = Vector3.Distance(transform.position, headPosition);
            targetLookPosition = headPosition;

            if (followPlayer)
                FollowPlayer();
            if (playerFollow)
                PlayerFollow();
            if (dynamicFov)
                DynamicFov();

            camera.fieldOfView = FieldOfView;
        }

        private void OnMove()
        {
        }

        private void OnRotation()
        {
        }


        private void FollowPlayer()
        {
            Vector3 positionAdjustment = Vector3.zero;

            float distance;

            if (targetDistance > maxDistance)
            {
                distance = (targetDistance - maxDistance);
                positionAdjustment += (transform.forward * distance);
            }

            if (followPlayer && playerFollow && targetDistance < minDistance)
            {
                distance = (minDistance - targetDistance);
                Vector3 previousPosition = transform.position;
                position -= transform.forward * distance;
                position.y = previousPosition.y;
                transform.position = position;
            }

            distance = targetDistance - (maxDistance - minDistance) / 5f - minDistance;
            if (distance > 0)
            {
                positionAdjustment += (transform.forward * distance) / 100f;
            }

            float yPositionAdjustment = (targetLookPosition.y - transform.position.y) / 25f;

            if (followPlayer && playerFollow && Mathf.Abs(yPositionAdjustment) > 0.001f)
            {
                Vector3 newPosition = transform.position;
                newPosition += Vector3.up * yPositionAdjustment;
                if (!Physics.Raycast(transform.position, (newPosition - transform.position),
                    Mathf.Abs(yPositionAdjustment)))
                {
                    positionAdjustment.y = newPosition.y - transform.position.y;
                }
            }

            if (positionAdjustment != Vector3.zero) transform.localPosition += positionAdjustment;
            transform.LookAt(targetLookPosition);
        }

        private void DynamicFov()
        {
            if (!followPlayer) return;
            Vector3 bodyPosition = targetPosition;
            Vector3 bodyVector = bodyPosition - position;
            Vector3 heightVector = bodyPosition + camera.transform.up * (targetHeight * 2) - position;
            FieldOfView = Vector3.Angle(heightVector, bodyVector);
        }

        private void PlayerFollow()
        {
            // Transform playerTransform = DropCamTweak.PlayerObject.transform;
            // Vector3 playerTransformRotation = playerTransform.eulerAngles;
            // playerTransform.LookAt(position);
            // playerTransformRotation.y = playerTransform.eulerAngles.y;
            // DropCamTweak.PlayerObject.transform.eulerAngles = playerTransformRotation;


            Transform lookTransform = DropCamTweak.PlayerHeadTarget.transform;
            Vector3 lookTransformRotation = lookTransform.eulerAngles;
            lookTransform.LookAt(position);
            float xDiff = Mathf.Clamp(Mathf.DeltaAngle(lookTransformRotation.x, lookTransform.eulerAngles.x), -90, 90);
            float yDiff = Mathf.Clamp(Mathf.DeltaAngle(lookTransformRotation.y, lookTransform.eulerAngles.y), -90, 90);
            float zDiff = Mathf.Clamp(Mathf.DeltaAngle(lookTransformRotation.z, lookTransform.eulerAngles.z), -90, 90);
            if (xDiff > -90 && xDiff < 90) lookTransformRotation.x += xDiff;
            if (yDiff > -90 && yDiff < 90) lookTransformRotation.y += yDiff;
            if (zDiff > -90 && zDiff < 90) lookTransformRotation.z += zDiff;
            DropCamTweak.PlayerHeadTarget.transform.eulerAngles = lookTransformRotation;


            // Quaternion lookTransform = (DropCamTweak.PlayerHeadTarget.transform.rotation);
            // lookTransform.SetLookRotation(position);
            // Quaternion lookRotation = DropCamTweak.PlayerHeadTarget.transform.rotation;
            // lookRotation.x = lookTransform.rotation.x;
            // lookRotation = lookTransform.rotation;
            // lookRotation.z = lookTransform.rotation.z;
            // DropCamTweak.PlayerHeadTarget.transform.rotation = lookTransform;
        }

        private void OnDisable()
        {
            followPlayer = false;
            playerFollow = false;
            dynamicFov = false;
        }

        private void OnApplicationQuit()
        {
            Application.wantsToQuit += () => !Input.GetKey("right alt");
        }
    }
}
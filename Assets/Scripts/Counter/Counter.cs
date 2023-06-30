using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace Pronotron.Modules
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Counter : MonoBehaviour
    {
        [SerializeField, Range(1, 9)]
        int digitCount = 7;

        [SerializeField, Range(0, 100000)]
        public int Current = 0;

        private Mesh _mesh;
        private Material _material;

        private float[] _currentDigits;

        void Awake()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
            _material = GetComponent<MeshRenderer>().sharedMaterial;
        }

        void OnEnable()
        {
            _material.SetInt("uNeedsAnimate", 0);
            _material.SetFloat("uDuration", 2.0f);
        }

        void OnDisable()
        {
            _material.SetInt("uNeedsAnimate", 0);
            Set(Current);
        }

        /// <summary>
        /// Set digits instantly to given number
        /// </summary>
        public void Set(int number = 0)
        {
            Current = number;
            _currentDigits = ShaderizeNumber(number);
            _material.SetFloatArray("uCurrentDigits", _currentDigits);
            _material.SetFloatArray("uTargetDigits", _currentDigits);
        }

        /// <summary>
        /// Calculates offset array of 2 integers and corrects direction of calculation
        /// </summary>
        public void AddWithAnimation(int number)
        {
            int targetNumber = Current + number;

            // Avoid negative
            if (targetNumber < 0) targetNumber = 0;

            // Difflength needed for adding multiplycation for next digit
            int diffLength = GetNumberLength(Mathf.Abs(Current - targetNumber));

            // Calculate animation offset to given target from current
            float[] targetDigits = ShaderizeNumber(targetNumber);
            float[] targetOffsets;

            if (Current < targetNumber)
            {
                targetOffsets = CalculateOffset(_currentDigits, targetDigits, diffLength);
            }
            else
            {
                targetOffsets = CalculateOffset(targetDigits, _currentDigits, diffLength);
                for (int i = 0; i < targetOffsets.Length; i++)
                {
                    targetOffsets[i] = -targetOffsets[i];
                }
            }

            // Apply to shader and start animation
            _material.SetFloatArray("uTargetDigits", targetDigits);
            _material.SetFloatArray("uTargetOffsets", targetOffsets);
            _material.SetFloat("uStartTime", Time.time);
            _material.SetInt("uNeedsAnimate", 1);

            // Apply inside
            StartCoroutine(FinishAnimation(targetNumber, targetDigits));
        }

        private IEnumerator FinishAnimation(int targetNumber, float[] targetDigits)
        {
            yield return new WaitForSeconds(2);
            _material.SetInt("uNeedsAnimate", 0);
            _material.SetFloatArray("uCurrentDigits", targetDigits);
            _currentDigits = targetDigits;
            Current = targetNumber;
        }

        /// <summary>
        /// Convert number to an array
        /// Add leading zeros and divide every digit by 10 to represent y position of the number on UV 
        /// example: 7325 = [0, 0, 0.7, 0.3, 0.2, 0.5]; for 6n digits
        /// </summary>
        private float[] ShaderizeNumber(int number)
        {
            string numberString = number.ToString().PadLeft(digitCount, '0');
            float[] result = new float[digitCount];

            for (int i = 0; i < digitCount; i++)
            {
                int digit = int.Parse(numberString[i].ToString());
                result[i] = (float)digit / 10;
            }

            // Convert leading zeros to -1 in order to represent an empty digit in shader, except last digit
            for (int j = 0; j < digitCount - 1; j++)
            {
                if (result[j] == 0.0f)
                {
                    result[j] = -1.0f;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Calculate offset between passed arrays (for 6n digit)
        /// example: 002560 -> 003562 = [0, 0, 0.1, 1.0, 10.0, 100.2]
        /// example: 0000999 -> 0001004 = [0, 0, 0, 0.1, 0.1, 0.1, 0.5]
        /// </summary>
        private float[] CalculateOffset(float[] arr1, float[] arr2, int diffLength)
        {
            float[] distances = new float[digitCount];
            float prevDistance = 0;

            for (int i = 0; i < digitCount; i++)
            {
                // Convert -1.0 to 0.0
                float diff = Mathf.Max(arr2[i], 0.0f) - Mathf.Max(arr1[i], 0.0f);

                if (diff < 0)
                {
                    diff += 1;
                }

                float dist = diff + prevDistance * 10;
                //float dist = (float)Math.Round(diff + prevDistance * 10, 1);
                
                distances[i] = dist;

                if (digitCount - i <= diffLength)
                {
                    prevDistance = dist;
                }
            }

            return distances;
        }

        /// <summary>
        /// Get the length of given number
        /// example: 243 = 3, 1000 = 4
        /// </summary>
        private int GetNumberLength(int number)
        {
            return (int)Mathf.Ceil(Mathf.Log10(number + 1));
        }

    }

}
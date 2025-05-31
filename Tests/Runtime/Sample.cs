using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Application.Runtime
{
    // https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/workflow-create-test.html
    public class Sample
    {
        [UnityTest]
        [Category("Sample")]
        public IEnumerator A()
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<Rigidbody>();
            var originalPosition = gameObject.transform.position.y;

            yield return new WaitForFixedUpdate();

            Assert.AreNotEqual(originalPosition, gameObject.transform.position.y);
        }
    }
}
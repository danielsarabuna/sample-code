using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Application.Editor
{
    // https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/workflow-create-test.html
    public class Sample : MonoBehaviour
    {
        [InitializeOnLoad]
        public class OnLoad
        {
            static OnLoad()
            {
                var editorIsOSX = false;
#if UNITY_EDITOR_OSX
                editorIsOSX = true;
#endif

                ConditionalIgnoreAttribute.AddConditionalIgnoreMapping("IgnoreInMacEditor", editorIsOSX);
            }
        }

        [UnityTest]
        [Category("Sample")]
        public IEnumerator A()
        {
            const bool condition = true;

            yield return null;

            Assert.IsTrue(condition);
        }

        [UnityTest]
        [Category("Sample")]
        public IEnumerator B()
        {
            var ints = new List<int>();
            for (var i = 0; i < 150; i++)
            {
                yield return null;
                ints.Add(Random.Range(0, 10));
            }

            Assert.IsTrue(ints.Contains(5));
        }

        [Test, ConditionalIgnore("IgnoreInMacEditor", "Ignored on Mac editor.")]
        [Category("EditorUtility")]
        public void C()
        {
            Assert.Pass();
        }
    }
}
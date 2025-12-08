using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.Sample
{
	/// <summary>
	/// Open up Window > General > Test Runner window
	/// Then go to the Play Mode tab and just double click on the test to run it.
	/// </summary>
	public class SamplePlayModeTest
	{
		[Test]
		public void SamplePlayModeTestSimplePasses()
		{
			// Use the Assert class to test conditions.
			
		}

		// A UnityTest behaves like a coroutine in PlayMode
		// and allows you to yield null to skip a frame in EditMode
		[UnityTest]
		public IEnumerator SamplePlayModeTestWithEnumeratorPasses()
		{
			// Use the Assert class to test conditions.
			// yield to skip a frame
			
			/* This will ignore unity debug logs - Otherwise test may fail accidentally
			 *
			 * Wwise has a habit of spamming the console
			 * with errors that are not relevant to the test
			 */
			LogAssert.ignoreFailingMessages = true;
			
			// Load the scene with index 1 (hub scene)
			SceneManager.LoadScene(1);
			
			// Wait 2 seconds
			yield return new WaitForSeconds(2f);
			
			// Try to get the player
			Player player = Object.FindFirstObjectByType<Player>();
			
			// Set the player's position high above the ground
			player.transform.position = new Vector3(0, 2000, 0);
			
			// Pass the test - Will return success
			Assert.Pass();
		}
	}
}
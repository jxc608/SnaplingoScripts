using UnityEngine;
using System.Collections;

namespace TA
{
	public class RhythmScaler : MonoBehaviour
	{
		#region [ --- Property --- ]
		public float scaleSize = 0.1f;
		Vector3 startScale;
		#endregion


		#region [ --- Mono --- ]
		void Start()
		{
			startScale = transform.localScale;
		}

        private void Update()
        {
            if(CorePlayManager.Instance)
                transform.localScale = CorePlayManager.Instance.RhythmBeat * scaleSize * Vector3.one + startScale;
        }

        #endregion
	}
	//RhythmScaler
}














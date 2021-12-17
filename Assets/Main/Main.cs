using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace Demo {

    public class Main : MonoBehaviour, IEnhancedScrollerDelegate {

        public EnhancedScroller scroller;
        public CellView cellView;

        public GameObject canvasRoot;
        public GameObject sclRoot;
        public GameObject backBtn;

        private List<Data> mDataLst = new List<Data>();

        void Start() {
            GameObject.DontDestroyOnLoad(canvasRoot);
            OnBtnShowScroller();

            scroller.Delegate = this;
            scroller.cellViewInstantiated = (scl, cv) => {
                CellView mycv = (CellView) cv;
                mycv.gameObject.SetActive(true);
                mycv.onClickFn = OnBtnItem;
            };

            InitData();
        }

        void InitData() {
            for (int i = 0; i < 50; i++) {
                mDataLst.Add(new Data { title = "aaa-" + i, path = "bbb" });
            }
        }

        public int GetNumberOfCells(EnhancedScroller scroller) {
            return mDataLst.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
            return 50;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
            CellView cv = (CellView) scroller.GetCellView(cellView);
            cv.SetData(mDataLst[dataIndex], dataIndex);
            return cv;
        }

        // UI 处理
        public void OnBtnItem(CellView cv, Data data) {
            sclRoot.SetActive(false);
            backBtn.SetActive(true);

            //

        }

        public void OnBtnShowScroller() {
            sclRoot.SetActive(true);
            backBtn.SetActive(false);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVP.Model;
using MVP.Presenter;

public class ProcessingStationTest : NoOpPresenter
{
   
    [SerializeField] private ProcessingStationPresenter presenter1;
    [SerializeField] private ProcessingStationPresenter presenter2;

        private void Start()
        {
            Init("test1", presenter1);
            Init("test2", presenter2);
        }

        private void Init(string id, ProcessingStationPresenter presenter)
        {
            var model = new ProcessingStationModel(id);
            presenter.InitPresenter(this);
            presenter.InitModel(model);
            presenter.ApplyCondition(ProcessingStationModel.START);
        }
    }

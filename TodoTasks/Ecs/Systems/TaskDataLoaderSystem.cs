using ECSFramework;

using Prometheus;

using todorest;

namespace TodoApp
{
    public class TaskDataLoaderSystem : ISystem<TaskEntity>
    {
        public string Name => "TaskDataLoaderSystem";

        private DataStore dataStore;


        private static readonly Counter entityLoadCount = Metrics
            .CreateCounter("entity_loaded_count", "Number of entities loaded by data loader.");
        
        public TaskDataLoaderSystem()
        {
            dataStore = DataStore.GetDataStore(1000);
        }

        public void Execute(AnEntityArchetype<TaskEntity> entityArchetype)
        {
            using (TodoMetrics.MethodMetrics("TaskDataLoaderSystem"))
            {
                var requestPool = ((ComponentPoolDod<TaskQuickViewRequestComponent>)entityArchetype
                                    .GetComponentPool(ComponentType.QUICK_VIEW_REQUEST))
                                    .GetActiveObjects();
                var quickViewTitlesPool = ((ComponentPoolDod<TaskQuickViewTitles>)entityArchetype
                                    .GetComponentPool(ComponentType.QUICK_VIEW_TASK_TITLES))
                                    .GetActiveObjects();

                var batchedRequest = GetBatchesToFetch(ref requestPool);

                var taskTitlesBatches = GetTaskTitles(batchedRequest);

                SetResponses(ref requestPool, ref quickViewTitlesPool, taskTitlesBatches);
            }
        }

        private Dictionary<int, int> GetBatchesToFetch(
            ref Span<TaskQuickViewRequestComponent> activeRequests)
        {
            //var batchedRequests = new ComponentPoolDod<TaskQuickViewRequestComponent>(requestPool.Length);

            var batchedRequests = new Dictionary<int, int>();

            for (int i = 0; i < activeRequests.Length; i++)
            {
                ref var request = ref activeRequests[i];

                var numberOfTasks = request.numberOfTasks;
                if (batchedRequests.ContainsKey(request.page))
                {
                    batchedRequests[request.page] = Math.Max(numberOfTasks, batchedRequests[request.page]);
                }
                else
                {
                    batchedRequests.Add(request.page, numberOfTasks);
                }
            }

            return batchedRequests;
        }

        private Dictionary<int, Memory<string>> GetTaskTitles(Dictionary<int, int> requestBatches)
        {
            var responseTitles = new Dictionary<int, Memory<string>>();
            foreach (var request in requestBatches)
            {
                var pageNumber = request.Key;
                var numberOfElements = request.Value;
                responseTitles.Add(pageNumber, dataStore.GetTaskTitles(pageNumber, numberOfElements));
            }

            return responseTitles;
        }

        private void SetResponses(
            ref Span<TaskQuickViewRequestComponent> requestSpan,
            ref Span<TaskQuickViewTitles> titleSpan,
            Dictionary<int, Memory<string>> responseBatches)
        {
            int rangeOfIteration = Math.Min(requestSpan.Length, titleSpan.Length);
            for (int i = 0; i < rangeOfIteration; i++)
            {
                ref var quickViewTitle = ref titleSpan[i];
                ref var request = ref requestSpan[i];

                if (quickViewTitle.IsSet
                    || request.IsSet
                    || !responseBatches.ContainsKey(request.page))
                    continue;

                quickViewTitle.taskTitle = responseBatches[request.page];

                quickViewTitle.IsSet = true;
                request.IsSet = true;
            }
        }

        public void Execute(AnEntityArchetype<TaskEntity> entityArchetype, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public void ExecuteBatch(AnEntityArchetype<TaskEntity> entityArchetype, int start, int end, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
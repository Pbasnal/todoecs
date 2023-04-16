using Prometheus;
using todorest;

namespace TodoApp
{
    public class TaskDataLoaderSystem : ISystem<TaskEntity>
    {
        private DataStore dataStore;

        private static readonly Counter entityLoadCount = Metrics
            .CreateCounter("entity_loaded_count", "Number of entities loaded by data loader.");

        public TaskDataLoaderSystem()
        {
            dataStore = DataStore.GetDataStore(1000);
        }

        public void Execute(EntityArchetype<TaskEntity> entityArchetype)
        {
            using (TodoMetrics.MethodMetrics("TaskDataLoaderSystem"))
            {
                var requestPool = entityArchetype.GetComponentPool<TaskQuickViewRequestComponent>();
                var batchedRequest = GetBatchesToFetch(entityArchetype, requestPool);

                var taskTitlesBatches = GetTaskTitles(batchedRequest);

                SetResponses(entityArchetype, taskTitlesBatches);
            }
        }

        private Dictionary<int, int> SetResponses(
            EntityArchetype<TaskEntity> entityArchetype,
            Dictionary<int, Memory<string>> responseBatches)
        {
            var requestPool = entityArchetype.GetComponentPool<TaskQuickViewRequestComponent>();
            var quickViewTitlesPool = entityArchetype.GetComponentPool<TaskQuickViewTitles>();

            var batchedRequests = new Dictionary<int, int>();
            foreach (var entity in entityArchetype.GetActiveEntities())
            {
                ref var quickViewTitle = ref quickViewTitlesPool.GetElementAt(entity.Index);
                if (quickViewTitle.IsSet) continue;

                ref var request = ref requestPool.GetElementAt(entity.Index);

                quickViewTitle.taskTitle = new string[request.numberOfTasks];
                if (!responseBatches.ContainsKey(request.page)) continue;

                var titles = responseBatches[request.page];
                for (int i = 0; i < quickViewTitle.taskTitle.Length && i < titles.Length; i++)
                {
                    quickViewTitle.taskTitle[i] = titles.Span[i];
                }

                quickViewTitle.IsSet = true;
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

        private Dictionary<int, int> GetBatchesToFetch(
            EntityArchetype<TaskEntity> entityArchetype,
            ComponentPool<TaskQuickViewRequestComponent> requestPool)
        {
            int numberOfEntitiesProcessed = 0;
            var batchedRequests = new Dictionary<int, int>();
            foreach (var entity in entityArchetype.GetActiveEntities())
            {
                numberOfEntitiesProcessed++;
                ref var request = ref requestPool.GetElementAt(entity.Index);

                if(request.IsSet) continue;

                var valueToAdd = request.numberOfTasks;
                if (batchedRequests.ContainsKey(request.page))
                {
                    valueToAdd = valueToAdd > batchedRequests[request.page] ?
                        valueToAdd : batchedRequests[request.page];

                    batchedRequests[request.page] = valueToAdd;
                }
                else
                {
                    batchedRequests.Add(request.page, valueToAdd);
                }
                request.IsSet = true;
            }
            entityLoadCount.Inc(numberOfEntitiesProcessed);

            return batchedRequests;
        }
    }
}
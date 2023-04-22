import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
  stages: [
    { duration: '5s', target: 100 },
    { duration: '10s', target: 1000 },
    { duration: '5m', target: 3000 },
    // { duration: '10s', target: 2000 },
    // { duration: '30s', target: 2000 },
  ]
};

export default function () {
  http.get('https://localhost:7158/TodoTasks');
  // http.get('https://localhost:7158/TodoTasksOop');
  sleep(1);
}
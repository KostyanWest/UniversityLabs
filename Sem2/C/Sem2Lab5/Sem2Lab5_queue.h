#ifndef SEM2LAB5_QUEUE_H
#define SEM2LAB5_QUEUE_H

typedef struct _QueueElement QueueElement;

struct _QueueElement
{
	int value;
	QueueElement* pNext;
	QueueElement* pPrev;
};

typedef struct _Queue
{
	QueueElement* pFirst;
	QueueElement* pLast;
} Queue;

void Enqueue(Queue* pQueue, int element);

int Dequeue(Queue* pQueue);

#endif // !SEM2LAB5_QUEUE_H
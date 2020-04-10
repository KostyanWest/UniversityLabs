CC=gcc
CFLAGS=-c -Wall
LDFLAGS=
SOURCES=Sem2Lab5.c Sem2Lab5_EPN.c Sem2Lab5_EMNIA.c Sem2Lab5_queue.c
LANGPOSTFIX=c
OBJECTS=$(SOURCES:.$(LANGPOSTFIX)=.o)
EXECUTABLE=Sem2Lab5.exe

.PHONY: all run build clean

all: build

run: build
	$(EXECUTABLE)

build: $(SOURCES) $(EXECUTABLE)
	
$(EXECUTABLE): $(OBJECTS) 
	$(CC) $(LDFLAGS) $(OBJECTS) -o $(EXECUTABLE)

%.o: %.$(LANGPOSTFIX)#				# Устаревшая запись ".$(LANGPOSTFIX).o:"
	$(CC) $(CFLAGS) $< -o $@

clean:
ifeq ($(OS),Windows_NT)
	del /q $(OBJECTS) $(EXECUTABLE)
else
	rm -rf $(OBJECTS) $(EXECUTABLE)
endif
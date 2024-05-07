default: build
build:
		echo "Building the project..."
		g++ -o snip src/main.cpp src/lexer.cpp
clean:
		echo "Cleaning up..."
		rm -f src/snip
test:
		echo "Running tests..."

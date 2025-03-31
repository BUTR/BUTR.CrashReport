FROM --platform=linux/amd64 mcr.microsoft.com/devcontainers/dotnet

RUN apt-get update && apt-get install -y --no-install-recommends \
    gcc-multilib \
    g++-multilib \
    gcc-mingw-w64-i686-posix \
    g++-mingw-w64-i686-posix \
    g++-mingw-w64-x86-64-posix \
    gcc-mingw-w64-x86-64-posix \
    && rm -rf /var/lib/apt/lists/*